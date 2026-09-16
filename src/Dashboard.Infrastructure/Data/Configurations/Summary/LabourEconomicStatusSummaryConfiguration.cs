using Dashboard.Domain.Entities.Summary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Persistence.Configurations.Summary;

public sealed class LabourEconomicStatusSummaryConfiguration
    : IEntityTypeConfiguration<LabourEconomicStatusSummary>
{
    public void Configure(
        EntityTypeBuilder<LabourEconomicStatusSummary> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("labour_economic_status_summary");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.SummaryId)
            .HasName("labour_economic_status_summary_pkey");

        builder.Property(x => x.SummaryId)
            .HasColumnName("summary_id")
            .ValueGeneratedOnAdd();

        // ============================================================
        // KY THONG KE
        // ============================================================

        builder.Property(x => x.ReferencePeriod)
            .HasColumnName("reference_period")
            .HasColumnType("date")
            .IsRequired();

        // ============================================================
        // DIMENSION KEYS
        // ============================================================

        builder.Property(x => x.AdministrativeUnitCode)
            .HasColumnName("administrative_unit_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EducationLevelCode)
            .HasColumnName("education_level_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TechnicalLevelCode)
            .HasColumnName("technical_level_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EconomicStatusCode)
            .HasColumnName("economic_status_code")
            .HasMaxLength(100)
            .IsRequired();

        // ============================================================
        // METRICS
        // ============================================================

        builder.Property(x => x.LabourCount)
            .HasColumnName("labour_count")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(x => x.ByPriority)
            .HasColumnName("by_priority")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        // D01-W04
        builder.Property(x => x.ByGender)
            .HasColumnName("by_gender")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        // D01-W07
        builder.Property(x => x.ByIndustry)
            .HasColumnName("by_industry")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        // D01-W05
        builder.Property(x => x.ByAgeGroup)
            .HasColumnName("by_age_group")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        // ============================================================
        // LINEAGE
        // ============================================================

        builder.Property(x => x.SourceIdentitySnapshotVersion)
            .HasColumnName("source_identity_snapshot_version")
            .IsRequired();

        builder.Property(x => x.SourceEmploymentSnapshotVersion)
            .HasColumnName("source_employment_snapshot_version")
            .IsRequired();

        builder.Property(x => x.AnalyticalDefinitionVersion)
            .HasColumnName("analytical_definition_version");

        builder.Property(x => x.ProcessingRunId)
            .HasColumnName("processing_run_id");

        builder.Property(x => x.AggregatedAt)
            .HasColumnName("aggregated_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // ============================================================
        // BUSINESS GRAIN
        // ============================================================

        builder.HasIndex(x => new
        {
            x.ReferencePeriod,
            x.AdministrativeUnitCode,
            x.EducationLevelCode,
            x.TechnicalLevelCode,
            x.EconomicStatusCode
        })
        .IsUnique()
        .HasDatabaseName(
            "uq_labour_economic_status_summary_grain");

        // ============================================================
        // FILTER INDEX
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AdministrativeUnitCode,
            x.EducationLevelCode,
            x.TechnicalLevelCode
        })
        .HasDatabaseName(
            "ix_labour_economic_status_summary_filter");

        // ============================================================
        // STATUS INDEX
        // ============================================================

        builder.HasIndex(x => x.EconomicStatusCode)
            .HasDatabaseName(
                "ix_labour_economic_status_summary_status");

        // ============================================================
        // ADMIN UNIT INDEX
        // ============================================================

        builder.HasIndex(x => x.AdministrativeUnitCode)
            .HasDatabaseName(
                "ix_labour_economic_status_summary_admin");

        // ============================================================
        // PERIOD INDEXES (D01-W01/W02)
        // ============================================================

        builder.HasIndex(x => x.ReferencePeriod)
            .HasDatabaseName(
                "ix_labour_economic_status_summary_period");

        builder.HasIndex(x => new
        {
            x.ReferencePeriod,
            x.AdministrativeUnitCode
        })
        .HasDatabaseName(
            "ix_labour_economic_status_summary_period_admin");
    }
}
