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

    public IFormFile? PhotoFile { get; set; }
    public IFormFile? SignatureFile { get; set; }
}
