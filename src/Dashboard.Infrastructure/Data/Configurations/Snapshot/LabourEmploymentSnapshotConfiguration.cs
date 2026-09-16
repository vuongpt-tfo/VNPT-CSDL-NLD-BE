using Dashboard.Domain.Entities.Snapshot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Persistence.Configurations.Snapshot;

public sealed class LabourEmploymentSnapshotConfiguration
    : IEntityTypeConfiguration<LabourEmploymentSnapshot>
{
    public void Configure(
        EntityTypeBuilder<LabourEmploymentSnapshot> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================
        // Schema được cấu hình tập trung tại DashboardDbContext:
        //
        //     modelBuilder.HasDefaultSchema(_schema);
        //
        // Configuration này chỉ khai báo tên bảng.

        builder.ToTable("labour_employment_snapshot");

        // ============================================================
        // PRIMARY KEY
        // ============================================================
        // Snapshot V1 chỉ giữ một trạng thái Employment hiện tại
        // cho mỗi người lao động.

        builder.HasKey(x => x.LabourId)
            .HasName("pk_labour_employment_snapshot");

        // ============================================================
        // IDENTITY
        // ============================================================

        builder.Property(x => x.LabourId)
            .HasColumnName("labour_id")
            .IsRequired();

        // ============================================================
        // BASIC
        // ============================================================

        builder.Property(x => x.SocialInsuranceNumber)
            .HasColumnName("social_insurance_number")
            .HasMaxLength(20);

        // ============================================================
        // EDUCATION / TRAINING / SKILL
        // ============================================================

        builder.Property(x => x.EducationLevelCode)
            .HasColumnName("education_level_code")
            .HasMaxLength(50);

        builder.Property(x => x.TechnicalLevelCode)
            .HasColumnName("technical_level_code")
            .HasMaxLength(50);

        builder.Property(x => x.MajorCode)
            .HasColumnName("major_code")
            .HasMaxLength(50);

        builder.Property(x => x.MajorDescription)
            .HasColumnName("major_description");

        builder.Property(x => x.SkillIndustryCode)
            .HasColumnName("skill_industry_code")
            .HasMaxLength(50);

        builder.Property(x => x.SkillLevelCode)
            .HasColumnName("skill_level_code")
            .HasMaxLength(50);

        // ============================================================
        // ECONOMIC ACTIVITY
        // ============================================================

        builder.Property(x => x.EconomicActivitiesCode)
            .HasColumnName("economic_activities_code")
            .IsRequired();

        // ============================================================
        // EMPLOYED
        // ============================================================

        builder.Property(x => x.PositionGroupsCode)
            .HasColumnName("position_groups_code")
            .HasMaxLength(50);

        builder.Property(x => x.OccupationCode)
            .HasColumnName("occupation_code")
            .HasMaxLength(50);

        builder.Property(x => x.ContractCode)
            .HasColumnName("contract_code")
            .HasMaxLength(50);

        // DATE -> semantic mapping to 03_time.dim_date.
        // Không tạo physical FK.

        builder.Property(x => x.EmploymentStatusDate)
            .HasColumnName("employment_status_date");

        builder.Property(x => x.WorkLocation)
            .HasColumnName("work_location")
            .HasMaxLength(200);

        builder.Property(x => x.JobTypesCode)
            .HasColumnName("job_types_code")
            .HasMaxLength(50);

        builder.Property(x => x.JobTitle)
            .HasColumnName("job_title")
            .HasMaxLength(200);

        builder.Property(x => x.TotalWorkExperience)
            .HasColumnName("total_work_experience");

        builder.Property(x => x.NumberOfJobs)
            .HasColumnName("number_of_jobs");

        builder.Property(x => x.EmployerCode)
            .HasColumnName("employer_code")
            .HasMaxLength(50);

        // PostgreSQL array type.

        builder.Property(x => x.IndustryCodes)
            .HasColumnName("industry_codes")
            .HasColumnType("text[]");

        builder.Property(x => x.IndustrialZoneStatus)
            .HasColumnName("industrial_zone_status");

        builder.Property(x => x.EmployerSectorsCode)
            .HasColumnName("employer_sectors_code")
            .HasMaxLength(50);

        // ============================================================
        // UNEMPLOYED
        // ============================================================

        builder.Property(x => x.EverWorked)
            .HasColumnName("ever_worked");

        // DATE -> semantic mapping to 03_time.dim_date.

        builder.Property(x => x.UnemploymentStartDate)
            .HasColumnName("unemployment_start_date");

        builder.Property(x => x.UnemploymentDuration)
            .HasColumnName("unemployment_duration");

        builder.Property(x => x.UnemploymentBenefitStatus)
            .HasColumnName("unemployment_benefit_status");

        builder.Property(x => x.UnemploymentReason)
            .HasColumnName("unemployment_reason");

        builder.Property(x => x.JobSearchWanted)
            .HasColumnName("job_search_wanted");

        builder.Property(x => x.JobSearchStatus)
            .HasColumnName("job_search_status");

        // ============================================================
        // NOT PARTICIPATING
        // ============================================================

        builder.Property(x => x.UneconomicReasonCode)
            .HasColumnName("uneconomic_reason_code")
            .HasMaxLength(50);

        // ============================================================
        // SOCIAL INSURANCE
        // ============================================================

        builder.Property(x => x.ParticipationFormCode)
            .HasColumnName("participation_form_code")
            .HasMaxLength(50);

        // DATE -> semantic mapping to 03_time.dim_date.

        builder.Property(x => x.SiStartDate)
            .HasColumnName("si_start_date");

        builder.Property(x => x.SiEndDate)
            .HasColumnName("si_end_date");

        // ============================================================
        // PRIORITY
        // ============================================================

        // PostgreSQL array type.

        builder.Property(x => x.PriorityCodes)
            .HasColumnName("priority_codes")
            .HasColumnType("text[]");

        // ============================================================
        // SNAPSHOT METADATA
        // ============================================================

        // Source freshness / synchronization watermark.
        // PostgreSQL: TIMESTAMPTZ.
        //
        // Có thể semantic-map phần date tới 03_time.dim_date
        // khi cần phân tích freshness.

        builder.Property(x => x.SourceUpdatedAt)
            .HasColumnName("source_updated_at");

        // Source/projection processing version.
        // Không liên quan tới Time Dimension.

        builder.Property(x => x.SnapshotVersion)
            .HasColumnName("snapshot_version")
            .IsRequired();

        // Technical write/update timestamp.
        //
        // PostgreSQL: TIMESTAMPTZ.
        //
        // Semantic mapping:
        //     DATE(snapshot_at) -> dim_date
        //     TIME(snapshot_at) -> dim_time
        //
        // Không tạo physical FK.

        builder.Property(x => x.SnapshotAt)
            .HasColumnName("snapshot_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // ============================================================
        // INDEXES
        // ============================================================

        builder.HasIndex(x => x.EconomicActivitiesCode)
            .HasDatabaseName(
                "ix_labour_employment_snapshot_economic");

        builder.HasIndex(x => x.OccupationCode)
            .HasDatabaseName(
                "ix_labour_employment_snapshot_occupation");

        builder.HasIndex(x => x.SkillIndustryCode)
            .HasDatabaseName(
                "ix_labour_employment_snapshot_industry");

        builder.HasIndex(x => x.EmployerCode)
            .HasDatabaseName(
                "ix_labour_employment_snapshot_employer");

        builder.HasIndex(x => x.SnapshotVersion)
            .HasDatabaseName(
                "ix_labour_employment_snapshot_version");
    }
}