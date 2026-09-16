using BuildingBlocks.CrossCutting.Logging;
using Dashboard.Application;
using Dashboard.Application.Abstractions.Aggregation;
using Dashboard.Application.Abstractions.Dimension;
using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Abstractions.Snapshots;
using Dashboard.Domain.Repositories;
using Dashboard.Infrastructure.Aggregates;
using Dashboard.Infrastructure.Data;
using Dashboard.Infrastructure.Dimensions;
using Dashboard.Infrastructure.Persistence.Repositories;
using Dashboard.Infrastructure.Snapshots;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
// BUILD
// ============================================================

var app = builder.Build();

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