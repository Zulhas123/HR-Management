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
    public DbSet<LeaveApprovalStep> LeaveApprovalSteps => Set<LeaveApprovalStep>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<WeekendConfiguration> WeekendConfigurations => Set<WeekendConfiguration>();
    public DbSet<LeaveEncashmentRequest> LeaveEncashmentRequests => Set<LeaveEncashmentRequest>();
    public DbSet<EmployeeOnboarding> EmployeeOnboardings => Set<EmployeeOnboarding>();
    public DbSet<EmployeeJoiningForm> EmployeeJoiningForms => Set<EmployeeJoiningForm>();
    public DbSet<OnboardingDocumentChecklistItem> OnboardingDocumentChecklistItems => Set<OnboardingDocumentChecklistItem>();
    public DbSet<OnboardingOrientationItem> OnboardingOrientationItems => Set<OnboardingOrientationItem>();
    public DbSet<EmployeeAssetAssignment> EmployeeAssetAssignments => Set<EmployeeAssetAssignment>();
    public DbSet<EmployeeHandbook> EmployeeHandbooks => Set<EmployeeHandbook>();
    public DbSet<EmployeeHandbookAcknowledgement> EmployeeHandbookAcknowledgements => Set<EmployeeHandbookAcknowledgement>();
    public DbSet<EmployeeOffboarding> EmployeeOffboardings => Set<EmployeeOffboarding>();
    public DbSet<ExitInterview> ExitInterviews => Set<ExitInterview>();
    public DbSet<OffboardingClearanceItem> OffboardingClearanceItems => Set<OffboardingClearanceItem>();
    public DbSet<FinalSettlement> FinalSettlements => Set<FinalSettlement>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<Religion> Religions => Set<Religion>();
    public DbSet<BloodGroup> BloodGroups => Set<BloodGroup>();

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
            b.Property(x => x.PassportNumber).HasMaxLength(50);
            b.Property(x => x.PresentAddress).HasMaxLength(500);
            b.Property(x => x.PermanentAddress).HasMaxLength(500);
            b.Property(x => x.BanglaFirstName).HasMaxLength(100);
            b.Property(x => x.BanglaLastName).HasMaxLength(100);
            b.Property(x => x.PhotoPath).HasMaxLength(500);
            b.Property(x => x.SignaturePath).HasMaxLength(500);
            b.Property(x => x.BankName).HasMaxLength(200);
            b.Property(x => x.BankAccountNumber).HasMaxLength(100);
            b.Property(x => x.MobileBankingProvider).HasMaxLength(100);
            b.Property(x => x.MobileBankingNumber).HasMaxLength(50);
            b.Property(x => x.BiometricUserId).HasMaxLength(100);
            b.Property(x => x.FaceProfileId).HasMaxLength(100);
            b.Property(x => x.RfidCardId).HasMaxLength(100);

            b.HasIndex(x => x.EmployeeCode).IsUnique();
            b.HasIndex(x => x.BiometricUserId);
            b.HasIndex(x => x.FaceProfileId);
            b.HasIndex(x => x.RfidCardId);

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

            b.HasOne(x => x.Religion)
                .WithMany()
                .HasForeignKey(x => x.ReligionId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.BloodGroup)
                .WithMany()
                .HasForeignKey(x => x.BloodGroupId)
                .OnDelete(DeleteBehavior.SetNull);
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
            b.Property(x => x.GraceMinutes).HasDefaultValue(0);
        });

        modelBuilder.Entity<AttendanceRecord>(b =>
        {
            b.Property(x => x.Notes).HasMaxLength(500);
            b.Property(x => x.DeviceId).HasMaxLength(100);
            b.Property(x => x.DeviceUserId).HasMaxLength(100);
            b.Property(x => x.RfidCardId).HasMaxLength(100);
            b.Property(x => x.MobileDeviceId).HasMaxLength(200);
            b.Property(x => x.CapturedBy).HasMaxLength(200);
            b.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
            b.Property(x => x.Longitude).HasColumnType("decimal(9,6)");

            b.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();
            b.HasIndex(x => x.Source);
            b.HasIndex(x => x.MissingPunchStatus);

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
            b.Property(x => x.MaxEncashmentDaysPerYear).HasColumnType("decimal(18,2)");
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

        modelBuilder.Entity<LeaveApprovalStep>(b =>
        {
            b.Property(x => x.DecidedBy).HasMaxLength(200);
            b.Property(x => x.Note).HasMaxLength(500);
            b.HasIndex(x => new { x.LeaveRequestId, x.Level }).IsUnique();

            b.HasOne(x => x.LeaveRequest)
                .WithMany(r => r.ApprovalSteps)
                .HasForeignKey(x => x.LeaveRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LeaveBalance>(b =>
        {
            b.Property(x => x.AllocatedDays).HasColumnType("decimal(18,2)");
            b.Property(x => x.UsedDays).HasColumnType("decimal(18,2)");
            b.Property(x => x.EncashmentDays).HasColumnType("decimal(18,2)");
            b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.LeaveType)
                .WithMany()
                .HasForeignKey(x => x.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Holiday>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.Date).IsUnique();
        });

        modelBuilder.Entity<WeekendConfiguration>(b =>
        {
            // allow multiple configs if needed later; MVP uses the latest
        });

        modelBuilder.Entity<LeaveEncashmentRequest>(b =>
        {
            b.Property(x => x.DaysRequested).HasColumnType("decimal(18,2)");
            b.Property(x => x.DecisionBy).HasMaxLength(200);
            b.Property(x => x.DecisionNote).HasMaxLength(500);
            b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year });

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.LeaveType)
                .WithMany()
                .HasForeignKey(x => x.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeeOnboarding>(b =>
        {
            b.HasIndex(x => x.EmployeeId);
            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeJoiningForm>(b =>
        {
            b.Property(x => x.Notes).HasMaxLength(2000);
            b.HasIndex(x => x.EmployeeOnboardingId).IsUnique();

            b.HasOne(x => x.EmployeeOnboarding)
                .WithOne(o => o.JoiningForm)
                .HasForeignKey<EmployeeJoiningForm>(x => x.EmployeeOnboardingId)
                .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<OnboardingDocumentChecklistItem>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Notes).HasMaxLength(1000);
            b.HasIndex(x => new { x.EmployeeOnboardingId, x.Name });

            b.HasOne(x => x.EmployeeOnboarding)
                .WithMany(o => o.DocumentChecklist)
                .HasForeignKey(x => x.EmployeeOnboardingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.EmployeeDocument)
                .WithMany()
                .HasForeignKey(x => x.EmployeeDocumentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OnboardingOrientationItem>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.CompletedBy).HasMaxLength(200);
            b.HasIndex(x => new { x.EmployeeOnboardingId, x.Title });

            b.HasOne(x => x.EmployeeOnboarding)
                .WithMany(o => o.OrientationChecklist)
                .HasForeignKey(x => x.EmployeeOnboardingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeAssetAssignment>(b =>
        {
            b.Property(x => x.AssetName).HasMaxLength(200).IsRequired();
            b.Property(x => x.AssetTag).HasMaxLength(100);
            b.Property(x => x.SerialNumber).HasMaxLength(100);
            b.Property(x => x.AssignedBy).HasMaxLength(200);
            b.Property(x => x.ReturnedTo).HasMaxLength(200);
            b.Property(x => x.ConditionOnAssign).HasMaxLength(500);
            b.Property(x => x.ConditionOnReturn).HasMaxLength(500);

            b.HasIndex(x => x.EmployeeId);
            b.HasIndex(x => new { x.EmployeeId, x.Status });

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeHandbook>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.FilePath).HasMaxLength(500).IsRequired();
            b.HasIndex(x => x.Title);
        });

        modelBuilder.Entity<EmployeeHandbookAcknowledgement>(b =>
        {
            b.HasIndex(x => new { x.EmployeeId, x.EmployeeHandbookId }).IsUnique();

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.EmployeeHandbook)
                .WithMany()
                .HasForeignKey(x => x.EmployeeHandbookId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeOffboarding>(b =>
        {
            b.Property(x => x.Reason).HasMaxLength(2000);
            b.HasIndex(x => x.EmployeeId);

            b.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExitInterview>(b =>
        {
            b.Property(x => x.Interviewer).HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(4000);
            b.HasIndex(x => x.EmployeeOffboardingId).IsUnique();

            b.HasOne(x => x.EmployeeOffboarding)
                .WithOne(o => o.ExitInterview)
                .HasForeignKey<ExitInterview>(x => x.EmployeeOffboardingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OffboardingClearanceItem>(b =>
        {
            b.Property(x => x.DepartmentName).HasMaxLength(200).IsRequired();
            b.Property(x => x.DecidedBy).HasMaxLength(200);
            b.Property(x => x.Note).HasMaxLength(1000);
            b.HasIndex(x => new { x.EmployeeOffboardingId, x.DepartmentName }).IsUnique();

            b.HasOne(x => x.EmployeeOffboarding)
                .WithMany(o => o.ClearanceItems)
                .HasForeignKey(x => x.EmployeeOffboardingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FinalSettlement>(b =>
        {
            b.Property(x => x.TotalPayable).HasColumnType("decimal(18,2)");
            b.Property(x => x.TotalDeductions).HasColumnType("decimal(18,2)");
            b.Property(x => x.NetPayable).HasColumnType("decimal(18,2)");
            b.Property(x => x.PreparedBy).HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(4000);
            b.HasIndex(x => x.EmployeeOffboardingId).IsUnique();

            b.HasOne(x => x.EmployeeOffboarding)
                .WithOne(o => o.FinalSettlement)
                .HasForeignKey<FinalSettlement>(x => x.EmployeeOffboardingId)
                .OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<Religion>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<BloodGroup>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(10).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });
    }
}
