using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence;

public sealed class HrSystemDbContext(DbContextOptions<HrSystemDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<EmploymentType> EmploymentTypes => Set<EmploymentType>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
    public DbSet<EmployeeEducation> EmployeeEducations => Set<EmployeeEducation>();
    public DbSet<EmployeeExperience> EmployeeExperiences => Set<EmployeeExperience>();
    public DbSet<EmployeeTransfer> EmployeeTransfers => Set<EmployeeTransfer>();
    public DbSet<EmployeePromotion> EmployeePromotions => Set<EmployeePromotion>();
    public DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts => Set<EmployeeEmergencyContact>();
    public DbSet<EmployeeFamilyMember> EmployeeFamilyMembers => Set<EmployeeFamilyMember>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Interview> Interviews => Set<Interview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Designation>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<EmploymentType>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Employee>(b =>
        {
            b.Property(x => x.EmployeeCode).HasMaxLength(32).IsRequired();
            b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Email).HasMaxLength(200);
            b.Property(x => x.Phone).HasMaxLength(50);
            b.Property(x => x.NidNumber).HasMaxLength(50);
            b.Property(x => x.TinNumber).HasMaxLength(50);
            b.Property(x => x.PhotoPath).HasMaxLength(500);
            b.Property(x => x.SignaturePath).HasMaxLength(500);
            b.Property(x => x.BankName).HasMaxLength(200);
            b.Property(x => x.BankAccountNumber).HasMaxLength(100);
            b.Property(x => x.MobileBankingProvider).HasMaxLength(100);
            b.Property(x => x.MobileBankingNumber).HasMaxLength(50);

            b.HasIndex(x => x.EmployeeCode).IsUnique();

            b.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Designation)
                .WithMany()
                .HasForeignKey(x => x.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.EmploymentType)
                .WithMany()
                .HasForeignKey(x => x.EmploymentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeeDocument>(b =>
        {
            b.Property(x => x.DocumentName).HasMaxLength(200).IsRequired();
            b.Property(x => x.DocumentType).HasMaxLength(100);
            b.Property(x => x.StoredPath).HasMaxLength(500).IsRequired();
            b.Property(x => x.OriginalFileName).HasMaxLength(255);

            b.HasIndex(x => new { x.EmployeeId, x.DocumentName });

            b.HasOne(x => x.Employee)
                .WithMany(e => e.Documents)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeEducation>(b =>
        {
            b.Property(x => x.Degree).HasMaxLength(200).IsRequired();
            b.Property(x => x.Institution).HasMaxLength(300);
            b.Property(x => x.Result).HasMaxLength(100);

            b.HasIndex(x => x.EmployeeId);

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeExperience>(b =>
        {
            b.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Designation).HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(1000);

            b.HasIndex(x => x.EmployeeId);

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeTransfer>(b =>
        {
            b.Property(x => x.Note).HasMaxLength(500);
            b.HasIndex(x => x.EmployeeId);

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.FromDepartment)
                .WithMany()
                .HasForeignKey(x => x.FromDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.ToDepartment)
                .WithMany()
                .HasForeignKey(x => x.ToDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<EmployeePromotion>(b =>
        {
            b.Property(x => x.Note).HasMaxLength(500);
            b.HasIndex(x => x.EmployeeId);

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.FromDesignation)
                .WithMany()
                .HasForeignKey(x => x.FromDesignationId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.ToDesignation)
                .WithMany()
                .HasForeignKey(x => x.ToDesignationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<EmployeeEmergencyContact>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Relationship).HasMaxLength(100);
            b.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            b.Property(x => x.Address).HasMaxLength(500);

            b.HasIndex(x => x.EmployeeId);

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeFamilyMember>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Relationship).HasMaxLength(100).IsRequired();
            b.Property(x => x.Phone).HasMaxLength(50);
            b.Property(x => x.Notes).HasMaxLength(500);

            b.HasIndex(x => x.EmployeeId);

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Shift>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<AttendanceRecord>(b =>
        {
            b.Property(x => x.Notes).HasMaxLength(500);

            b.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Shift)
                .WithMany()
                .HasForeignKey(x => x.ShiftId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LeaveType>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500);
            b.HasIndex(x => x.Name).IsUnique();
            b.Property(x => x.DefaultAnnualAllocation).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<LeaveRequest>(b =>
        {
            b.Property(x => x.Reason).HasMaxLength(500);
            b.Property(x => x.DecisionBy).HasMaxLength(200);
            b.Property(x => x.DecisionNote).HasMaxLength(500);
            b.Property(x => x.TotalDays).HasColumnType("decimal(18,2)");

            b.HasIndex(x => new { x.EmployeeId, x.StartDate, x.EndDate });

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.LeaveType)
                .WithMany()
                .HasForeignKey(x => x.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JobPosting>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Department).HasMaxLength(200);
            b.Property(x => x.Location).HasMaxLength(200);
            b.Property(x => x.EmploymentType).HasMaxLength(100);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.HasIndex(x => x.Title);
        });

        modelBuilder.Entity<Candidate>(b =>
        {
            b.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Email).HasMaxLength(200);
            b.Property(x => x.Phone).HasMaxLength(50);
            b.Property(x => x.CvUrl).HasMaxLength(500);
            b.Property(x => x.Notes).HasMaxLength(2000);
            b.HasIndex(x => x.Email);
        });

        modelBuilder.Entity<JobApplication>(b =>
        {
            b.Property(x => x.Notes).HasMaxLength(2000);
            b.HasIndex(x => new { x.JobPostingId, x.CandidateId }).IsUnique();

            b.HasOne(x => x.JobPosting)
                .WithMany()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Interview>(b =>
        {
            b.Property(x => x.Interviewer).HasMaxLength(200);
            b.Property(x => x.Mode).HasMaxLength(50);
            b.Property(x => x.Feedback).HasMaxLength(2000);

            b.HasOne(x => x.JobApplication)
                .WithMany()
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
