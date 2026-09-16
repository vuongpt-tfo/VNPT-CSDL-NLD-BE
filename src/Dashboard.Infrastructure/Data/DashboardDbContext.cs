using Dashboard.Domain.Entities.Dimension;
using Dashboard.Domain.Entities.Dimension.Analytical;
using Dashboard.Domain.Entities.Snapshot;
using Dashboard.Domain.Entities.Summary;

using Dashboard.Infrastructure.Data.Configurations.Dimension;
using Dashboard.Infrastructure.Data.Configurations.Dimension.Analytical;
using Dashboard.Infrastructure.Persistence.Configurations.Snapshot;
using Dashboard.Infrastructure.Persistence.Configurations.Summary;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dashboard.Infrastructure.Data;

public sealed class DashboardDbContext : DbContext
{
    private readonly string _schema;

    // Public read-only property để các thành phần khác
    // có thể truy cập schema hiện tại.
    public string Schema => _schema;

    public DashboardDbContext(
        DbContextOptions<DashboardDbContext> options,
        IConfiguration config)
        : base(options)
    {
        // Ví dụ:
        // "Database:Schema": "dashboard_sch"
        _schema = config["Database:Schema"] ?? "dashboard_sch";
    }

    // ============================================================
    // DIMENSIONS - REFERENCE
    // ============================================================

    public DbSet<DimCatalogItem> DimCatalogItems
        => Set<DimCatalogItem>();

    public DbSet<DimAgeRange> DimAgeRanges
        => Set<DimAgeRange>();

    public DbSet<DimAdministrativeUnit> DimAdministrativeUnits
        => Set<DimAdministrativeUnit>();

    public DbSet<DimIndustry> DimIndustries
        => Set<DimIndustry>();

    public DbSet<DimOccupation> DimOccupations
        => Set<DimOccupation>();

    // ============================================================
    // DIMENSIONS - ANALYTICAL
    // ============================================================

    public DbSet<DimAnalytical> DimAnalyticals
        => Set<DimAnalytical>();

    public DbSet<DimAnalyticalItem> DimAnalyticalItems
        => Set<DimAnalyticalItem>();

    public DbSet<DimAnalyticalItemTranslation> DimAnalyticalItemTranslations
        => Set<DimAnalyticalItemTranslation>();

    public DbSet<DimAnalyticalRule> DimAnalyticalRules
        => Set<DimAnalyticalRule>();

    public DbSet<DimAnalyticalRuleCondition> DimAnalyticalRuleConditions
        => Set<DimAnalyticalRuleCondition>();

    // ============================================================
    // SNAPSHOTS
    // ============================================================

    public DbSet<LabourIdentitySnapshot> LabourIdentitySnapshots
        => Set<LabourIdentitySnapshot>();

    public DbSet<LabourEmploymentSnapshot> LabourEmploymentSnapshots
        => Set<LabourEmploymentSnapshot>();

    // ============================================================
    // SUMMARIES
    // ============================================================

    public DbSet<LabourIdentitySummary> LabourIdentitySummaries
        => Set<LabourIdentitySummary>();

    public DbSet<LabourEconomicStatusSummary> LabourEconomicStatusSummaries
        => Set<LabourEconomicStatusSummary>();

    public DbSet<LabourTimeSummary> LabourTimeSummaries
        => Set<LabourTimeSummary>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        // ========================================================
        // DEFAULT SCHEMA
        // ========================================================

        modelBuilder.HasDefaultSchema(_schema);

        base.OnModelCreating(modelBuilder);

        // ========================================================
        // DIMENSIONS - REFERENCE
        // ========================================================

        modelBuilder.ApplyConfiguration(
            new DimCatalogItemConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimAgeRangeConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimAdministrativeUnitConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimIndustryConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimOccupationConfiguration());

        // ========================================================
        // DIMENSIONS - ANALYTICAL
        // ========================================================

        modelBuilder.ApplyConfiguration(
            new DimAnalyticalConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimAnalyticalItemConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimAnalyticalItemTranslationConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimAnalyticalRuleConfiguration());

        modelBuilder.ApplyConfiguration(
            new DimAnalyticalRuleConditionConfiguration());

        // ========================================================
        // SNAPSHOTS
        // ========================================================

        modelBuilder.ApplyConfiguration(
            new LabourIdentitySnapshotConfiguration());

        modelBuilder.ApplyConfiguration(
            new LabourEmploymentSnapshotConfiguration());

        // ========================================================
        // SUMMARIES
        // ========================================================

        modelBuilder.ApplyConfiguration(
            new LabourIdentitySummaryConfiguration());

        modelBuilder.ApplyConfiguration(
            new LabourEconomicStatusSummaryConfiguration());

        modelBuilder.ApplyConfiguration(
            new LabourTimeSummaryConfiguration());
    }
}