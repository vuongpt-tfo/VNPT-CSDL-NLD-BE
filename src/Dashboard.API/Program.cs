using BuildingBlocks.CrossCutting.Logging;
using Dashboard.Application;
using Dashboard.Application.Abstractions.Aggregation;
using Dashboard.Application.Abstractions.Cache;
using Dashboard.Application.Abstractions.Dimension;
using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Abstractions.Snapshots;
using Dashboard.Domain.Repositories;
using Dashboard.Infrastructure.Aggregates;
using Dashboard.Infrastructure.Data;
using Dashboard.Infrastructure.Data.Seed;
using Dashboard.Infrastructure.Dimensions;
using Dashboard.Infrastructure.Persistence.Repositories;
using Dashboard.Infrastructure.Redis;
using Dashboard.Infrastructure.Snapshots;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// SERILOG
// ============================================================

builder.AddSerilogLogging();

// ============================================================
// SERVICES
// ============================================================

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("DashboardFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================================================
// DATABASE
// ============================================================

var connectionString =
    builder.Configuration.GetConnectionString("DashboardConnection");

builder.Services.AddDbContextPool<DashboardDbContext>(options =>
    options
        .UseNpgsql(
            connectionString,
            npgsql =>
            {
                npgsql.CommandTimeout(300);
                npgsql.EnableRetryOnFailure(5);
            })
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
);

// ============================================================
// MEDIATR
// ============================================================

builder.Services.AddMediatR(typeof(AssemblyMarker).Assembly);

// ============================================================
// REPOSITORIES
// ============================================================

builder.Services.AddScoped<
    ILabourDashboardReadRepository,
    LabourDashboardReadRepository>();

// ============================================================
// AGGREGATION
// ============================================================
//
// Tang tinh Summary tu Snapshot (Aggregate). Chua co co che trigger
// (scheduled worker / Kafka consumer / endpoint - chua chot, xem
// docs/decisions/decision-log.md) - IAggregationService chi moi duoc
// dang ky de co the goi thu cong/tu noi khac khi can.
//
// SnapshotRepository implement ca ISnapshotReader (Application, ban
// ghi da join, dung cho Aggregate) lan ISnapshotRepository (Domain,
// entity tho) - dang ky rieng 2 dong vi la 2 interface khac nhau.

builder.Services.AddScoped<ISnapshotReader, SnapshotRepository>();
builder.Services.AddScoped<ISnapshotRepository, SnapshotRepository>();
builder.Services.AddScoped<IDimensionReader, DimensionRepository>();
builder.Services.AddScoped<ISummaryRepository, SummaryRepository>();
builder.Services.AddScoped<IAggregationService, LabourAggregationService>();

// ============================================================
// REDIS CACHE
// ============================================================
//
// Redis la performance cache (xem database/05_summary/README.md muc
// 26 - Summary and Redis), khong phai source of truth. AbortOnConnectFail
// = false de app van khoi dong duoc ngay ca khi Redis chua san sang -
// DashboardRedisCache tu bat loi ket noi/thao tac va coi nhu cache-miss
// (xem Redis/DashboardRedisCache.cs). Chi dang ky khi co cau hinh
// "Redis:ConnectionString" - CHUA co handler/repository nao trong
// LabourDashboard goi IDashboardCache (xem docs/decisions/decision-log.md D7).

var redisConnectionString =
    builder.Configuration["Redis:ConnectionString"];

if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
    redisOptions.AbortOnConnectFail = false;

    builder.Services.AddSingleton<IConnectionMultiplexer>(
        _ => ConnectionMultiplexer.Connect(redisOptions));

    builder.Services.AddScoped<IDashboardCache, DashboardRedisCache>();
}

// ============================================================
// BUILD
// ============================================================

var app = builder.Build();

// ============================================================
// DATABASE MIGRATION + DEMO SEED (chi Development/Local)
// ============================================================
//
// Chi tu-migrate/seed o Development/Local de tranh dong DDL/du lieu
// demo len moi truong SIT/UAT/Production ngoai y muon (xem
// docs/decisions/decision-log.md D5). "Database:SeedDemoData" mac
// dinh false (khong khai bao trong appsettings.json) - chi bat trong
// appsettings.Local.json cho dev ca nhan.

if (app.Environment.IsDevelopment()
    || app.Environment.EnvironmentName == "Local")
{
    using var migrationScope = app.Services.CreateScope();

    var dbContext = migrationScope.ServiceProvider
        .GetRequiredService<DashboardDbContext>();

    await dbContext.Database.MigrateAsync();

    if (app.Configuration.GetValue<bool>("Database:SeedDemoData"))
    {
        await DashboardDemoDataSeeder.SeedAsync(dbContext);
    }
}

// ============================================================
// HTTP PIPELINE
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ============================================================
// CORS
// ============================================================

app.UseCors("DashboardFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

try
{
    app.MapControllers();
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var loaderException in ex.LoaderExceptions)
    {
        Console.WriteLine("========== LOADER EXCEPTION ==========");
        Console.WriteLine(loaderException?.ToString());
    }

    throw;
}

app.Run();