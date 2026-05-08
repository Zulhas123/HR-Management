using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        services.AddDbContext<HrSystemDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IRepository<Department>, Repository<Department>>();
        services.AddScoped<IRepository<Designation>, Repository<Designation>>();
        services.AddScoped<IRepository<EmploymentType>, Repository<EmploymentType>>();
        services.AddScoped<IRepository<Employee>, EmployeeRepository>();
        services.AddScoped<IRepository<Shift>, Repository<Shift>>();
        services.AddScoped<IRepository<AttendanceRecord>, AttendanceRecordRepository>();

        return services;
    }
}
