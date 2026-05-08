using HrSystem.Application;
using HrSystem.Application.Security;
using HrSystem.Infrastructure;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];
        var key = jwt["Key"];

        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience) || string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Jwt configuration is missing (Issuer, Audience, Key).");
        }

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HrSystemDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.EmploymentTypes.AnyAsync())
    {
        db.EmploymentTypes.AddRange(
            new HrSystem.Domain.Entities.EmploymentType { Name = "Permanent" },
            new HrSystem.Domain.Entities.EmploymentType { Name = "Contractual" },
            new HrSystem.Domain.Entities.EmploymentType { Name = "Intern" },
            new HrSystem.Domain.Entities.EmploymentType { Name = "Part-time" });
    }

    if (!await db.Shifts.AnyAsync())
    {
        db.Shifts.AddRange(
            new HrSystem.Domain.Entities.Shift { Name = "General", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0), IsOvernight = false, RequiredWorkMinutes = 480 },
            new HrSystem.Domain.Entities.Shift { Name = "Night", StartTime = new TimeOnly(22, 0), EndTime = new TimeOnly(6, 0), IsOvernight = true, RequiredWorkMinutes = 480 });
    }

    if (!await db.LeaveTypes.AnyAsync())
    {
        db.LeaveTypes.AddRange(
            new HrSystem.Domain.Entities.LeaveType { Name = "Casual Leave", DefaultAnnualAllocation = 10, IsPaid = true, ApprovalLevelsRequired = 2 },
            new HrSystem.Domain.Entities.LeaveType { Name = "Sick Leave", DefaultAnnualAllocation = 14, IsPaid = true, ApprovalLevelsRequired = 1 },
            new HrSystem.Domain.Entities.LeaveType { Name = "Earn Leave", DefaultAnnualAllocation = 0, IsPaid = true, ApprovalLevelsRequired = 2, AllowEncashment = true, MaxEncashmentDaysPerYear = 10 },
            new HrSystem.Domain.Entities.LeaveType { Name = "Maternity Leave", DefaultAnnualAllocation = 0, IsPaid = true, ApprovalLevelsRequired = 2 },
            new HrSystem.Domain.Entities.LeaveType { Name = "Paternity Leave", DefaultAnnualAllocation = 0, IsPaid = true, ApprovalLevelsRequired = 2 },
            new HrSystem.Domain.Entities.LeaveType { Name = "Festival Leave", DefaultAnnualAllocation = 0, IsPaid = true, ApprovalLevelsRequired = 1 },
            new HrSystem.Domain.Entities.LeaveType { Name = "Leave Without Pay", DefaultAnnualAllocation = 0, IsPaid = false, ApprovalLevelsRequired = 1 });
    }

    if (!await db.WeekendConfigurations.AnyAsync())
    {
        db.WeekendConfigurations.Add(new HrSystem.Domain.Entities.WeekendConfiguration
        {
            Friday = true,
            Saturday = true
        });
    }

    if (!await db.JobPostings.AnyAsync())
    {
        db.JobPostings.Add(new HrSystem.Domain.Entities.JobPosting
        {
            Title = "Software Engineer",
            Department = "Engineering",
            Location = "Dhaka",
            EmploymentType = "Permanent",
            Description = "MVP seeded job posting.",
            IsOpen = true
        });
    }

    if (!await db.Religions.AnyAsync())
    {
        db.Religions.AddRange(
            new HrSystem.Domain.Entities.Religion { Name = "Islam" },
            new HrSystem.Domain.Entities.Religion { Name = "Hinduism" },
            new HrSystem.Domain.Entities.Religion { Name = "Christianity" },
            new HrSystem.Domain.Entities.Religion { Name = "Buddhism" },
            new HrSystem.Domain.Entities.Religion { Name = "Other" });
    }

    if (!await db.BloodGroups.AnyAsync())
    {
        db.BloodGroups.AddRange(
            new HrSystem.Domain.Entities.BloodGroup { Name = "A+" },
            new HrSystem.Domain.Entities.BloodGroup { Name = "A-" },
            new HrSystem.Domain.Entities.BloodGroup { Name = "B+" },
            new HrSystem.Domain.Entities.BloodGroup { Name = "B-" },
            new HrSystem.Domain.Entities.BloodGroup { Name = "AB+" },
            new HrSystem.Domain.Entities.BloodGroup { Name = "AB-" },
            new HrSystem.Domain.Entities.BloodGroup { Name = "O+" },
            new HrSystem.Domain.Entities.BloodGroup { Name = "O-" });
    }

    if (!await db.OvertimePolicies.AnyAsync())
    {
        db.OvertimePolicies.Add(new HrSystem.Domain.Entities.OvertimePolicy
        {
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            NormalMultiplier = 1.0m,
            HolidayMultiplier = 2.0m,
            ApprovalLevelsRequired = 1
        });
    }

    // RBAC seed (roles, permissions, Super Admin user)
    if (!await db.AppPermissions.AnyAsync())
    {
        db.AppPermissions.AddRange(
            new HrSystem.Domain.Entities.AppPermission { Code = "employees.read", Description = "Read employees" },
            new HrSystem.Domain.Entities.AppPermission { Code = "employees.write", Description = "Create/update/delete employees" },
            new HrSystem.Domain.Entities.AppPermission { Code = "audit.read", Description = "Read audit logs and login history" },
            new HrSystem.Domain.Entities.AppPermission { Code = "security.manage", Description = "Manage users, roles, and permissions" });
    }

    if (!await db.AppRoles.AnyAsync())
    {
        db.AppRoles.AddRange(
            new HrSystem.Domain.Entities.AppRole { Name = "Super Admin", Description = "Full system access" },
            new HrSystem.Domain.Entities.AppRole { Name = "HR Admin", Description = "HR administration" },
            new HrSystem.Domain.Entities.AppRole { Name = "HR Manager", Description = "HR management" },
            new HrSystem.Domain.Entities.AppRole { Name = "Team Lead", Description = "Team lead" },
            new HrSystem.Domain.Entities.AppRole { Name = "Employee", Description = "Employee self access" },
            new HrSystem.Domain.Entities.AppRole { Name = "Accounts", Description = "Accounts/payroll access" },
            new HrSystem.Domain.Entities.AppRole { Name = "Branch Manager", Description = "Branch management" });
    }

    await db.SaveChangesAsync();

    var superAdminRole = await db.AppRoles.FirstOrDefaultAsync(x => x.Name == "Super Admin");
    if (superAdminRole is not null)
    {
        var permissionIds = await db.AppPermissions.Select(x => x.Id).ToListAsync();
        foreach (var pid in permissionIds)
        {
            if (!await db.AppRolePermissions.AnyAsync(x => x.AppRoleId == superAdminRole.Id && x.AppPermissionId == pid))
            {
                db.AppRolePermissions.Add(new HrSystem.Domain.Entities.AppRolePermission
                {
                    AppRoleId = superAdminRole.Id,
                    AppPermissionId = pid
                });
            }
        }
    }

    var adminUser = builder.Configuration["Admin:Username"] ?? "admin";
    var adminPass = builder.Configuration["Admin:Password"] ?? "admin123";

    var existingAdmin = await db.AppUsers.FirstOrDefaultAsync(x => x.Username == adminUser);
    if (existingAdmin is null)
    {
        existingAdmin = new HrSystem.Domain.Entities.AppUser
        {
            Username = adminUser,
            DisplayName = adminUser,
            PasswordHash = PasswordHasher.Hash(adminPass),
            IsActive = true
        };

        db.AppUsers.Add(existingAdmin);
        await db.SaveChangesAsync();
    }

    if (superAdminRole is not null)
    {
        var hasRole = await db.AppUserRoles.AnyAsync(x => x.AppUserId == existingAdmin.Id && x.AppRoleId == superAdminRole.Id);
        if (!hasRole)
        {
            db.AppUserRoles.Add(new HrSystem.Domain.Entities.AppUserRole
            {
                AppUserId = existingAdmin.Id,
                AppRoleId = superAdminRole.Id
            });
        }
    }

    await db.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<HrSystem.Web.Middleware.RequestAuditMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
