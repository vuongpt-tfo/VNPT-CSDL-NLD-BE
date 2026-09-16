using Dashboard.Domain.Entities.Snapshot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Persistence.Configurations.Snapshot;

public sealed class LabourIdentitySnapshotConfiguration
    : IEntityTypeConfiguration<LabourIdentitySnapshot>
{
    public void Configure(
        EntityTypeBuilder<LabourIdentitySnapshot> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================
        // Schema được cấu hình tập trung tại DashboardDbContext:
        //
        //     modelBuilder.HasDefaultSchema(_schema);
        //
        // Configuration này chỉ khai báo tên bảng.

        builder.ToTable("labour_identity_snapshot");

        // ============================================================
        // PRIMARY KEY
        // ============================================================
        // PostgreSQL:
        //
        //     PRIMARY KEY (labour_id, date_of_birth)
        //
        // date_of_birth đồng thời là partition key của bảng.

        builder.HasKey(x => new
        {
            x.LabourId,
            x.DateOfBirth
        })
        .HasName("pk_labour_identity_snapshot");

        // ============================================================
        // IDENTITY
        // ============================================================

        builder.Property(x => x.LabourId)
            .HasColumnName("labour_id")
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        // ============================================================
        // PERSONAL IDENTITY
        // ============================================================

        builder.Property(x => x.CccdNumber)
            .HasColumnName("cccd_number")
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(x => x.CmndNumber)
            .HasColumnName("cmnd_number")
            .HasMaxLength(12);

        builder.Property(x => x.FullName)
            .HasColumnName("full_name")
            .IsRequired();

        builder.Property(x => x.AccType)
            .HasColumnName("acc_type")
            .HasMaxLength(20)
            .IsRequired();

        // ============================================================
        // PERSONAL INFORMATION
        // ============================================================

        builder.Property(x => x.GenderCode)
            .HasColumnName("gender_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IssueDate)
            .HasColumnName("issue_date");

        builder.Property(x => x.IssuePlace)
            .HasColumnName("issue_place");

        builder.Property(x => x.ExpiryDate)
            .HasColumnName("expiry_date");

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        builder.Property(x => x.EthnicCode)
            .HasColumnName("ethnic_code")
            .HasMaxLength(50);

        builder.Property(x => x.ReligionCode)
            .HasColumnName("religion_code")
            .HasMaxLength(50);

        // ============================================================
        // ADDRESS
        // ============================================================

        builder.Property(x => x.PermanentAddressCode)
            .HasColumnName("permanent_address_code")
            .HasMaxLength(50);

        builder.Property(x => x.PermanentAddressDetail)
            .HasColumnName("permanent_address_detail");

        builder.Property(x => x.CurrentAddressCode)
            .HasColumnName("current_address_code")
            .HasMaxLength(50);

        builder.Property(x => x.CurrentAddressDetail)
            .HasColumnName("current_address_detail");

        // ============================================================
        // CONTACT / FAMILY
        // ============================================================

        builder.Property(x => x.Email)
            .HasColumnName("email");

        builder.Property(x => x.MaritalStatus)
            .HasColumnName("marital_status")
            .HasMaxLength(20);

        // ============================================================
        // SNAPSHOT METADATA
        // ============================================================

        // Thời điểm dữ liệu nguồn được cập nhật.
        // Dùng cho freshness / synchronization / watermark.
        //
        // PostgreSQL: TIMESTAMPTZ

        builder.Property(x => x.SourceUpdatedAt)
            .HasColumnName("source_updated_at");

        // Version của source/projection đã được xử lý.
        // Không phải Catalog Version và không phải business history.

        builder.Property(x => x.SnapshotVersion)
            .HasColumnName("snapshot_version")
            .IsRequired();

        // Thời điểm technical row được ghi/cập nhật vào Snapshot.
        // Không phải analytical/business effective date.
        //
        // Có thể semantic-map:
        //     DATE(snapshot_at) -> 03_time.dim_date
        //     TIME(snapshot_at) -> 03_time.dim_time
        //
        // Không tạo physical FK tới Time Dimension.

        builder.Property(x => x.SnapshotAt)
            .HasColumnName("snapshot_at")
            .IsRequired();
    }
}