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
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IRepository<EmployeeDocument>, EmployeeDocumentRepository>();
        services.AddScoped<IRepository<EmployeeEducation>, Repository<EmployeeEducation>>();
        services.AddScoped<IRepository<EmployeeExperience>, Repository<EmployeeExperience>>();
        services.AddScoped<IRepository<EmployeeTransfer>, EmployeeTransferRepository>();
        services.AddScoped<IRepository<EmployeePromotion>, EmployeePromotionRepository>();
        services.AddScoped<IRepository<EmployeeEmergencyContact>, Repository<EmployeeEmergencyContact>>();
        services.AddScoped<IRepository<EmployeeFamilyMember>, Repository<EmployeeFamilyMember>>();
        services.AddScoped<IRepository<Shift>, Repository<Shift>>();
        services.AddScoped<IRepository<AttendanceRecord>, AttendanceRecordRepository>();
        services.AddScoped<IAttendanceRecordRepository, AttendanceRecordRepository>();
        services.AddScoped<IRepository<LeaveType>, Repository<LeaveType>>();
        services.AddScoped<IRepository<LeaveRequest>, LeaveRequestRepository>();
        services.AddScoped<IRepository<JobPosting>, Repository<JobPosting>>();
        services.AddScoped<IRepository<Candidate>, Repository<Candidate>>();
        services.AddScoped<IRepository<JobApplication>, JobApplicationRepository>();
        services.AddScoped<IRepository<Interview>, InterviewRepository>();
        services.AddScoped<IRepository<Religion>, Repository<Religion>>();
        services.AddScoped<IRepository<BloodGroup>, Repository<BloodGroup>>();

        return services;
    }
}
