using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Employee : BaseEntity
{
    public required string EmployeeCode { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public DateOnly JoinDate { get; set; }
    public DateOnly? ResignationDate { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int DesignationId { get; set; }
    public Designation? Designation { get; set; }

    public int EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }

    public string? NidNumber { get; set; }
    public string? TinNumber { get; set; }
    public string? PassportNumber { get; set; }

    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }

    public int? ReligionId { get; set; }
    public Religion? Religion { get; set; }

    public int? BloodGroupId { get; set; }
    public BloodGroup? BloodGroup { get; set; }

    public bool IsFestivalEligible { get; set; } = true;

    public string? BanglaFirstName { get; set; }
    public string? BanglaLastName { get; set; }

    public string? PhotoPath { get; set; }
    public string? SignaturePath { get; set; }

    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? MobileBankingProvider { get; set; }
    public string? MobileBankingNumber { get; set; }

    // Attendance device identifiers (optional)
    public string? BiometricUserId { get; set; }
    public string? FaceProfileId { get; set; }
    public string? RfidCardId { get; set; }

    public List<EmployeeDocument> Documents { get; set; } = [];
}
