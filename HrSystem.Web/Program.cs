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
