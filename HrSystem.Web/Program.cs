using HrSystem.Application;
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
            new HrSystem.Domain.Entities.Shift { Name = "General", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0), IsOvernight = false },
            new HrSystem.Domain.Entities.Shift { Name = "Night", StartTime = new TimeOnly(22, 0), EndTime = new TimeOnly(6, 0), IsOvernight = true });
    }

    if (!await db.LeaveTypes.AnyAsync())
    {
        db.LeaveTypes.AddRange(
            new HrSystem.Domain.Entities.LeaveType { Name = "Casual Leave", DefaultAnnualAllocation = 10, IsPaid = true },
            new HrSystem.Domain.Entities.LeaveType { Name = "Sick Leave", DefaultAnnualAllocation = 14, IsPaid = true },
            new HrSystem.Domain.Entities.LeaveType { Name = "Earn Leave", DefaultAnnualAllocation = 0, IsPaid = true },
            new HrSystem.Domain.Entities.LeaveType { Name = "Maternity Leave", DefaultAnnualAllocation = 0, IsPaid = true },
            new HrSystem.Domain.Entities.LeaveType { Name = "Paternity Leave", DefaultAnnualAllocation = 0, IsPaid = true },
            new HrSystem.Domain.Entities.LeaveType { Name = "Festival Leave", DefaultAnnualAllocation = 0, IsPaid = true },
            new HrSystem.Domain.Entities.LeaveType { Name = "Leave Without Pay", DefaultAnnualAllocation = 0, IsPaid = false });
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
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
