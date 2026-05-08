using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeFormVm
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(100)]
    public string LastName { get; set; } = "";

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime JoinDate { get; set; }

    [Required]
    public int DepartmentId { get; set; }
    public List<SelectListItem> Departments { get; set; } = [];

    [Required]
    public int DesignationId { get; set; }
    public List<SelectListItem> Designations { get; set; } = [];

    [Required]
    public int EmploymentTypeId { get; set; }
    public List<SelectListItem> EmploymentTypes { get; set; } = [];

    [StringLength(50)]
    public string? NidNumber { get; set; }

    [StringLength(50)]
    public string? TinNumber { get; set; }

    [StringLength(50)]
    public string? PassportNumber { get; set; }

    [StringLength(500)]
    public string? PresentAddress { get; set; }

    [StringLength(500)]
    public string? PermanentAddress { get; set; }

    public int? ReligionId { get; set; }
    public List<SelectListItem> Religions { get; set; } = [];

    public int? BloodGroupId { get; set; }
    public List<SelectListItem> BloodGroups { get; set; } = [];

    public bool IsFestivalEligible { get; set; } = true;

    [StringLength(100)]
    public string? BanglaFirstName { get; set; }

    [StringLength(100)]
    public string? BanglaLastName { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ResignationDate { get; set; }

    [StringLength(200)]
    public string? BankName { get; set; }

    [StringLength(100)]
    public string? BankAccountNumber { get; set; }

    [StringLength(100)]
    public string? MobileBankingProvider { get; set; }

    [StringLength(50)]
    public string? MobileBankingNumber { get; set; }

    // Attendance device identifiers (optional)
    [StringLength(100)]
    public string? BiometricUserId { get; set; }

    [StringLength(100)]
    public string? FaceProfileId { get; set; }

    [StringLength(100)]
    public string? RfidCardId { get; set; }

    public IFormFile? PhotoFile { get; set; }
    public IFormFile? SignatureFile { get; set; }
}
