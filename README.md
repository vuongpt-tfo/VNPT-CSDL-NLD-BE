# HƯỚNG DẪN PROJECT DASHBOARD

## 1. Mục tiêu

Tạo project theo kiến trúc:

```text
src/
├── Dashboard.API/
├── Dashboard.Application/
├── Dashboard.Domain/
└── Dashboard.Infrastructure/
```

Mẫu đầu tiên cần chạy được API:

```text
GET /api/v1/dashboard/labour/by-age
```

Luồng:

```text
Controller
    ↓
MediatR Query
    ↓
Handler
    ↓
Repository Interface
    ↓
Infrastructure Repository
    ↓
dashboard_sch
```

Sau khi API mẫu chạy ổn mới mở rộng:

```text
Snapshot
    ↓
Aggregate
    ↓
Summary
    ↓
Redis
    ↓
Dashboard API
```

---

## 2. Tạo Solution và Project

Tại thư mục `src/`:

```bash
dotnet new sln -n Dashboard

dotnet new webapi -n Dashboard.API
dotnet new classlib -n Dashboard.Application
dotnet new classlib -n Dashboard.Domain
dotnet new classlib -n Dashboard.Infrastructure
```

Thêm project vào Solution:

```bash
dotnet sln Dashboard.sln add Dashboard.API
dotnet sln Dashboard.sln add Dashboard.Application
dotnet sln Dashboard.sln add Dashboard.Domain
dotnet sln Dashboard.sln add Dashboard.Infrastructure
```

---

## 3. Project Dependency

Quan hệ dependency:

```text
Dashboard.API
      │
      ├──────────────► Dashboard.Application
      │
      └──────────────► Dashboard.Infrastructure
                            │
                            ├──► Dashboard.Application
                            └──► Dashboard.Domain

Dashboard.Application
      │
      └──────────────► Dashboard.Domain

Dashboard.Domain
      │
      └── Không phụ thuộc project khác
```

Tạo reference:

```bash
dotnet add Dashboard.Application reference Dashboard.Domain

dotnet add Dashboard.Infrastructure reference Dashboard.Application
dotnet add Dashboard.Infrastructure reference Dashboard.Domain

dotnet add Dashboard.API reference Dashboard.Application
dotnet add Dashboard.API reference Dashboard.Infrastructure
```

---

## 4. Cấu trúc code ban đầu

```text
Dashboard.API/
├── Controllers/
│   └── LabourDashboardController.cs
├── DTOs/
│   └── LabourIdentity/
├── Extensions/
│   └── ServiceCollectionExtensions.cs
├── Mappers/
├── Middleware/
├── Program.cs
└── appsettings.json

Dashboard.Application/
├── Abstractions/
│   ├── Aggregation/
│   ├── Cache/
│   ├── Dimension/
│   ├── Persistence/
│   └── Snapshots/
├── Common/
│   ├── Dtos/
│   └── Models/
└── Features/
    └── LabourDashboard/
        ├── Dtos/
        └── Queries/
            └── GetLabourByAge/
                ├── GetLabourByAgeQuery.cs
                └── GetLabourByAgeQueryHandler.cs

Dashboard.Domain/
├── Entities/
├── Enums/
├── Repositories/
├── Rules/
└── ValueObjects/

Dashboard.Infrastructure/
├── Aggregates/
├── Data/
├── Dimensions/
├── Extensions/
├── Persistence/
├── Queries/
├── Redis/
└── Snapshots/
```

Không cần tạo toàn bộ class ngay từ đầu. Feature nào triển khai thì tạo folder/class tương ứng.

---

## 5. Cài NuGet Package

### Dashboard.Infrastructure

```bash
dotnet add Dashboard.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add Dashboard.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add Dashboard.Infrastructure package StackExchange.Redis
```

### Dashboard.Application

```bash
dotnet add Dashboard.Application package MediatR
```

### Dashboard.API

```bash
dotnet add Dashboard.API package MediatR
dotnet add Dashboard.API package Serilog.AspNetCore
dotnet add Dashboard.API package Serilog.Sinks.Console
dotnet add Dashboard.API package Serilog.Sinks.File
dotnet add Dashboard.API package Serilog.Formatting.Compact
```

Nếu `BuildingBlocks` của hệ thống đã cung cấp các package tương ứng thì sử dụng lại, không cài trùng.

---

## 6. Cấu hình appsettings.json

Team tự tạo `Dashboard.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DashboardConnection": "Username=<db-user>;Password=<db-password>;Host=<db-host>;Port=5432;Database=<db-name>;Pooling=true;"
  },

  "Database": {
    "Schema": "dashboard_sch"
  },

  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File"
    ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithThreadId",
      "WithEnvironmentName"
    ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "Serilog.Formatting.Compact.RenderedCompactJsonFormatter, Serilog.Formatting.Compact"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.json",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "formatter": "Serilog.Formatting.Compact.RenderedCompactJsonFormatter, Serilog.Formatting.Compact",
          "restrictedToMinimumLevel": "Debug",
          "shared": true
        }
      }
    ],
    "Properties": {
      "Application": "DashboardService"
    }
  }
}
```

> Lưu ý: đây là placeholder minh họa cấu trúc, không phải giá trị thật. Không commit Connection String thật (user/password/host) vào Git dưới bất kỳ hình thức nào — kể cả trong README hay appsettings.*.json đã commit. Lấy giá trị thật từ Secret Manager / Environment Variable của từng môi trường (Dev/UAT/Prod).

---

## 7. Tạo DashboardDbContext

Tạo:

```text
Dashboard.Infrastructure/
└── Data/
    └── DashboardDbContext.cs
```

```csharp
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Data;

public class DashboardDbContext : DbContext
{
    public DashboardDbContext(
        DbContextOptions<DashboardDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dashboard_sch");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DashboardDbContext).Assembly);
    }
}
```

Sau này mở rộng theo:

```text
Entities
    ↓
Configurations
    ↓
DashboardDbContext
```

---

## 8. Tạo Application AssemblyMarker

Tạo:

```text
Dashboard.Application/
└── AssemblyMarker.cs
```

```csharp
namespace Dashboard.Application;

public sealed class AssemblyMarker
{
}
```

Mục đích là dùng Assembly để MediatR scan Handler.

---

## 9. Tạo API Controller

Tạo:

```text
Dashboard.API/
└── Controllers/
    └── LabourDashboardController.cs
```

```csharp
using BuildingBlocks.SharedKernel.Common;
using Dashboard.Application.Common.Dtos;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByAge;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.API.Controllers;

[ApiController]
[Route("api/v1/dashboard/labour")]
//[Authorize]
public class LabourDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public LabourDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================================
    // GET api/v1/dashboard/labour/by-age
    // ============================================================

    [HttpGet("by-age")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourAgeDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ByAge(
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourByAgeQuery(),
            ct);

        return Ok(ApiResult.Ok(result));
    }
}
```

Endpoint:

```text
GET /api/v1/dashboard/labour/by-age
```

---

## 10. Tạo Query

Tạo:

```text
Dashboard.Application/
└── Features/
    └── LabourDashboard/
        └── Queries/
            └── GetLabourByAge/
                └── GetLabourByAgeQuery.cs
```

```csharp
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByAge;

public sealed record GetLabourByAgeQuery
    : IRequest<IReadOnlyList<LabourAgeDto>>;
```

---

## 11. Tạo DTO

Tạo:

```text
Dashboard.Application/
└── Common/
    └── Dtos/
        └── LabourAgeDto.cs
```

```csharp
namespace Dashboard.Application.Common.Dtos;

public sealed class LabourAgeDto
{
    public string AgeRangeCode { get; init; } = default!;

    public string AgeRangeName { get; init; } = default!;

    public long LabourCount { get; init; }
}
```

---

## 12. Tạo Repository Abstraction

Application chỉ định nghĩa interface.

Tạo:

```text
Dashboard.Application/
└── Abstractions/
    └── Persistence/
        └── ILabourDashboardReadRepository.cs
```

```csharp
using Dashboard.Application.Common.Dtos;

namespace Dashboard.Application.Abstractions.Persistence;

public interface ILabourDashboardReadRepository
{
    Task<IReadOnlyList<LabourAgeDto>> GetLabourByAgeAsync(
        CancellationToken cancellationToken);
}
```

Application không biết PostgreSQL, EF Core hay SQL cụ thể.

---

## 13. Tạo Query Handler

Tạo:

```text
Dashboard.Application/
└── Features/
    └── LabourDashboard/
        └── Queries/
            └── GetLabourByAge/
                └── GetLabourByAgeQueryHandler.cs
```

```csharp
using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByAge;

public sealed class GetLabourByAgeQueryHandler
    : IRequestHandler<
        GetLabourByAgeQuery,
        IReadOnlyList<LabourAgeDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourByAgeQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourAgeDto>> Handle(
        GetLabourByAgeQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetLabourByAgeAsync(
            cancellationToken);
    }
}
```

Handler không viết SQL trực tiếp.

---

## 14. Tạo Repository Implementation

Tạo:

```text
Dashboard.Infrastructure/
└── Persistence/
    └── Repositories/
        └── LabourDashboardReadRepository.cs
```

Repository implement:

```csharp
ILabourDashboardReadRepository
```

Luồng:

```text
Application
    │
    │ interface
    ▼
ILabourDashboardReadRepository
    ▲
    │ implementation
    │
Infrastructure
    │
    ▼
DashboardDbContext
    │
    ▼
dashboard_sch
```

Repository đọc dữ liệu phục vụ Dashboard từ:

```text
dashboard_sch
├── dimension
├── snapshot
└── summary
```

Không để Dashboard API query trực tiếp:

```text
nlabour_sch.labour_identity
```

để thực hiện Dashboard aggregate.

---

## 15. Đăng ký Infrastructure DI

Tạo:

```text
Dashboard.Infrastructure/
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

```csharp
using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Infrastructure.Data;
using Dashboard.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "DashboardConnection");

        services.AddDbContext<DashboardDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<
            ILabourDashboardReadRepository,
            LabourDashboardReadRepository>();

        return services;
    }
}
```

---

## 16. Cấu hình Program.cs

`Dashboard.API/Program.cs`:

```csharp
using Dashboard.Application;
using Dashboard.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(AssemblyMarker).Assembly);
});

builder.Services.AddDashboardInfrastructure(
    builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
```

---

## 17. Chạy project

Tại thư mục `src`:

```bash
dotnet restore
dotnet build
dotnet run --project Dashboard.API
```

Mở Swagger:

```text
/swagger
```

Test API:

```text
GET /api/v1/dashboard/labour/by-age
```

Hoặc:

```bash
curl https://localhost:xxxx/api/v1/dashboard/labour/by-age
```

---

## 18. Luồng hoàn chỉnh của API mẫu

```text
HTTP GET
    │
    ▼
LabourDashboardController
    │
    ▼
GetLabourByAgeQuery
    │
    ▼
MediatR
    │
    ▼
GetLabourByAgeQueryHandler
    │
    ▼
ILabourDashboardReadRepository
    │
    ▼
LabourDashboardReadRepository
    │
    ▼
DashboardDbContext
    │
    ▼
dashboard_sch
    │
    ▼
LabourAgeDto
    │
    ▼
ApiResult
    │
    ▼
HTTP Response
```

---

## 19. Thứ tự triển khai khuyến nghị

Team không cần dựng toàn bộ hệ thống ngay.

Thực hiện theo thứ tự:

```text
01. Tạo Solution
        ↓
02. Tạo 4 Project
        ↓
03. Tạo Project Reference
        ↓
04. Tạo cấu trúc folder
        ↓
05. Cấu hình appsettings.json
        ↓
06. DashboardDbContext
        ↓
07. DI
        ↓
08. MediatR
        ↓
09. Controller
        ↓
10. Query
        ↓
11. Handler
        ↓
12. Repository
        ↓
13. PostgreSQL Query
        ↓
14. Test API
        ↓
15. Redis
        ↓
16. Aggregate
        ↓
17. Snapshot/Worker
```

Mẫu đầu tiên chỉ cần đạt:

```text
GET /api/v1/dashboard/labour/by-age
            │
            ▼
       trả JSON thành công
```

Sau khi mẫu API chạy ổn mới triển khai đầy đủ:

```text
SNAPSHOT
    ↓
AGGREGATE
    ↓
SUMMARY
    ↓
REDIS
    ↓
DASHBOARD API
    ↓
DASHBOARD UI
```

## Tài liệu & Change Log

Quy ước ghi log thay đổi (decision log, changelog, audit report, review report) nằm ở `docs/README.md` — xem đó trước khi thêm tài liệu mới, đừng chép lại quy tắc vào file này.
