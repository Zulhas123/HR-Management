using HrSystem.Application.Abstractions;
using HrSystem.Application.Services;
using HrSystem.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HrSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IAttendancePunchService, AttendancePunchService>();
        services.AddScoped<IAttendanceProcessingService, AttendanceProcessingService>();
        services.AddScoped<ILeaveCalendarService, LeaveCalendarService>();
        services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
        services.AddScoped<IPayrollIntegrationService, PayrollIntegrationService>();
        services.AddScoped<IWorkforceReportingService, WorkforceReportingService>();
        services.AddScoped<IOvertimeService, OvertimeService>();
        services.AddScoped<IAuthService, AuthService>();

        // Convenience typed registrations for common lookups
        services.AddScoped<ICrudService<Department>, CrudService<Department>>();
        services.AddScoped<ICrudService<Designation>, CrudService<Designation>>();
        services.AddScoped<ICrudService<EmploymentType>, CrudService<EmploymentType>>();
        services.AddScoped<ICrudService<EmployeeDocument>, CrudService<EmployeeDocument>>();
        services.AddScoped<ICrudService<EmployeeEducation>, CrudService<EmployeeEducation>>();
        services.AddScoped<ICrudService<EmployeeExperience>, CrudService<EmployeeExperience>>();
        services.AddScoped<ICrudService<EmployeeTransfer>, CrudService<EmployeeTransfer>>();
        services.AddScoped<ICrudService<EmployeePromotion>, CrudService<EmployeePromotion>>();
        services.AddScoped<ICrudService<EmployeeEmergencyContact>, CrudService<EmployeeEmergencyContact>>();
        services.AddScoped<ICrudService<EmployeeFamilyMember>, CrudService<EmployeeFamilyMember>>();
        services.AddScoped<ICrudService<Shift>, CrudService<Shift>>();
        services.AddScoped<ICrudService<AttendanceRecord>, CrudService<AttendanceRecord>>();
        services.AddScoped<ICrudService<LeaveType>, CrudService<LeaveType>>();
        services.AddScoped<ICrudService<LeaveRequest>, CrudService<LeaveRequest>>();
        services.AddScoped<ICrudService<JobPosting>, CrudService<JobPosting>>();
        services.AddScoped<ICrudService<Candidate>, CrudService<Candidate>>();
        services.AddScoped<ICrudService<JobApplication>, CrudService<JobApplication>>();
        services.AddScoped<ICrudService<Interview>, CrudService<Interview>>();
        services.AddScoped<ICrudService<Religion>, CrudService<Religion>>();
        services.AddScoped<ICrudService<BloodGroup>, CrudService<BloodGroup>>();
        services.AddScoped<ICrudService<EmployeeOnboarding>, CrudService<EmployeeOnboarding>>();
        services.AddScoped<ICrudService<EmployeeJoiningForm>, CrudService<EmployeeJoiningForm>>();
        services.AddScoped<ICrudService<OnboardingDocumentChecklistItem>, CrudService<OnboardingDocumentChecklistItem>>();
        services.AddScoped<ICrudService<OnboardingOrientationItem>, CrudService<OnboardingOrientationItem>>();
        services.AddScoped<ICrudService<EmployeeAssetAssignment>, CrudService<EmployeeAssetAssignment>>();
        services.AddScoped<ICrudService<EmployeeHandbook>, CrudService<EmployeeHandbook>>();
        services.AddScoped<ICrudService<EmployeeHandbookAcknowledgement>, CrudService<EmployeeHandbookAcknowledgement>>();
        services.AddScoped<ICrudService<EmployeeOffboarding>, CrudService<EmployeeOffboarding>>();
        services.AddScoped<ICrudService<ExitInterview>, CrudService<ExitInterview>>();
        services.AddScoped<ICrudService<OffboardingClearanceItem>, CrudService<OffboardingClearanceItem>>();
        services.AddScoped<ICrudService<FinalSettlement>, CrudService<FinalSettlement>>();
        services.AddScoped<ICrudService<EmployeeBonus>, CrudService<EmployeeBonus>>();
        services.AddScoped<ICrudService<SalaryAdjustment>, CrudService<SalaryAdjustment>>();
        services.AddScoped<ICrudService<EmployeeTask>, CrudService<EmployeeTask>>();
        services.AddScoped<ICrudService<DailyWorkLog>, CrudService<DailyWorkLog>>();
        services.AddScoped<ICrudService<OvertimeRequest>, CrudService<OvertimeRequest>>();
        services.AddScoped<ICrudService<OvertimePolicy>, CrudService<OvertimePolicy>>();

        return services;
    }
}
