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
        services.AddScoped<ICrudService<Shift>, CrudService<Shift>>();
        services.AddScoped<ICrudService<AttendanceRecord>, CrudService<AttendanceRecord>>();

        return services;
    }
}
