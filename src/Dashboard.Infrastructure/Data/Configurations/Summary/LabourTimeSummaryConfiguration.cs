using Dashboard.Domain.Entities.Summary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Persistence.Configurations.Summary;

public sealed class LabourTimeSummaryConfiguration
    : IEntityTypeConfiguration<LabourTimeSummary>
{
    public void Configure(
        EntityTypeBuilder<LabourTimeSummary> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("labour_time_summary");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.SummaryId)
            .HasName("labour_time_summary_pkey");

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

        // ============================================================
        // METRICS
        // ============================================================

        builder.Property(x => x.WorkingAgeLabourCount)
            .HasColumnName("working_age_labour_count")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(x => x.OutWorkingAgeLabourCount)
            .HasColumnName("out_working_age_labour_count")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(x => x.TotalLabourCount)
            .HasColumnName("total_labour_count")
            .HasDefaultValue(0L)
            .IsRequired();

        // ============================================================
        // REVISION / BACKFILL (D01-W08)
        // ============================================================

        builder.Property(x => x.PreviousWorkingAgeLabourCount)
            .HasColumnName("previous_working_age_labour_count");

        builder.Property(x => x.PreviousOutWorkingAgeLabourCount)
            .HasColumnName("previous_out_working_age_labour_count");

        builder.Property(x => x.PreviousTotalLabourCount)
            .HasColumnName("previous_total_labour_count");

        builder.Property(x => x.IsRevised)
            .HasColumnName("is_revised")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.RevisedAt)
            .HasColumnName("revised_at")
            .HasColumnType("timestamp with time zone");

        // ============================================================
        // QUY TAC PHAN LOAI
        // ============================================================

        builder.Property(x => x.AnalyticalDefinitionVersion)
            .HasColumnName("analytical_definition_version");

        builder.Property(x => x.WorkingAgeFrom)
            .HasColumnName("working_age_from")
            .HasDefaultValue((short)15)
            .IsRequired();

        builder.Property(x => x.WorkingAgeTo)
            .HasColumnName("working_age_to")
            .HasDefaultValue((short)62)
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
        //   reference_period
        //   administrative_unit
        //   education_level
        //   technical_level
        //

        builder.HasIndex(x => new
        {
            x.ReferencePeriod,
            x.AdministrativeUnitCode,
            x.EducationLevelCode,
            x.TechnicalLevelCode
        })
        .IsUnique()
        .HasDatabaseName(
            "uq_labour_time_summary_grain");

        // ============================================================
        // FILTER / LOOKUP INDEXES (mirror database/05_summary/labour_time_summary.sql)
        // ============================================================

        builder.HasIndex(x => new
        {
            x.ReferencePeriod,
            x.AdministrativeUnitCode
        })
        .HasDatabaseName(
            "ix_labour_time_summary_period_admin");

        builder.HasIndex(x => new
        {
            x.AdministrativeUnitCode,
            x.EducationLevelCode,
            x.TechnicalLevelCode
        })
        .HasDatabaseName(
            "ix_labour_time_summary_filter");

        builder.HasIndex(x => x.ReferencePeriod)
            .HasDatabaseName(
                "ix_labour_time_summary_period");

        builder.HasIndex(x => x.IsRevised)
            .HasDatabaseName(
                "ix_labour_time_summary_revised");
    }
}
