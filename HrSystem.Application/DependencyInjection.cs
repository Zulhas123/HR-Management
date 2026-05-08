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

        return services;
    }
}
