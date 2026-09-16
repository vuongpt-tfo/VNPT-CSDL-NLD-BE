using Dashboard.Domain.Entities.Summary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Persistence.Configurations.Summary;

public sealed class LabourIdentitySummaryConfiguration
    : IEntityTypeConfiguration<LabourIdentitySummary>
{
    public void Configure(
        EntityTypeBuilder<LabourIdentitySummary> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("labour_identity_summary");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.SummaryId)
            .HasName("labour_identity_summary_pkey");

        builder.Property(x => x.SummaryId)
            .HasColumnName("summary_id")
            .ValueGeneratedOnAdd();

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

        builder.Property(x => x.TopLevelOccupationCode)
            .HasColumnName("top_level_occupation_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.AgeGroupCode)
            .HasColumnName("age_group_code")
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
        builder.Property(x => x.MaleCount)
            .HasColumnName("male_count");

        // D01-W07
        builder.Property(x => x.ByIndustry)
            .HasColumnName("by_industry")
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
        //
        // Grain:
        //   administrative_unit
        //   education_level
        //   technical_level
        //   top_level_occupation
        //   age_group
        //   economic_status
        //

        builder.HasIndex(x => new
        {
            x.AdministrativeUnitCode,
            x.EducationLevelCode,
            x.TechnicalLevelCode,
            x.TopLevelOccupationCode,
            x.AgeGroupCode,
            x.EconomicStatusCode
        })
        .IsUnique()
        .HasDatabaseName(
            "uq_labour_identity_summary_grain");

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
            "ix_labour_identity_summary_filter");

        // ============================================================
        // AGE INDEX
        // ============================================================

        builder.HasIndex(x => x.AgeGroupCode)
            .HasDatabaseName(
                "ix_labour_identity_summary_age");

        // ============================================================
        // OCCUPATION INDEX
        // ============================================================

        builder.HasIndex(x => x.TopLevelOccupationCode)
            .HasDatabaseName(
                "ix_labour_identity_summary_occupation");

        // ============================================================
        // ECONOMIC STATUS INDEX
        // ============================================================

        builder.HasIndex(x => x.EconomicStatusCode)
            .HasDatabaseName(
                "ix_labour_identity_summary_status");
    }
}