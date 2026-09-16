namespace Dashboard.Domain.Entities.Snapshot;

public class LabourIdentitySnapshot
{
    public long LabourId { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string CccdNumber { get; set; } = null!;

    public string? CmndNumber { get; set; }

    public string FullName { get; set; } = null!;

    public string AccType { get; set; } = null!;

    public string GenderCode { get; set; } = null!;

    public DateTime? IssueDate { get; set; }

    public string? IssuePlace { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? PhoneNumber { get; set; }

    public string? EthnicCode { get; set; }

    public string? ReligionCode { get; set; }

    public string? PermanentAddressCode { get; set; }

    public string? PermanentAddressDetail { get; set; }

    public string? CurrentAddressCode { get; set; }

    public string? CurrentAddressDetail { get; set; }

    public string? Email { get; set; }

    public string? MaritalStatus { get; set; }

    public DateTimeOffset? SourceUpdatedAt { get; set; }

    public long SnapshotVersion { get; set; }

    public DateTimeOffset SnapshotAt { get; set; }
}