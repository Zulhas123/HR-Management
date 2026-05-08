# HR Management System (ASP.NET Core MVC)

Bangladesh-focused HRMS with a minimal, maintainable implementation:

- ASP.NET Core MVC (UI) + REST APIs
- Clean Architecture layering: `Domain` / `Application` / `Infrastructure` / `Web`
- Repository pattern + DI
- EF Core (code-first) + SQL Server
- JWT authentication + RBAC (roles/permissions)
- Docker Compose for local SQL Server + app

## Architecture

- `HrSystem.Domain`: Entities + repository abstractions (no EF/UI concerns)
- `HrSystem.Application`: Use-case services (business logic, async)
- `HrSystem.Infrastructure`: EF Core `DbContext` + repository implementations
- `HrSystem.Web`: MVC UI + API controllers + auth/configuration

## Folder Structure (Feature-Oriented)

- `HrSystem.Domain/Entities/<Feature>/...`: Feature-grouped entities (Attendance, Leave, Recruitment, Onboarding, Offboarding, Workforce, PayrollIntegration, Overtime, Security, MasterData)
- `HrSystem.Domain/Repositories/<Feature>/...`: Feature-grouped repository abstractions
- `HrSystem.Application/Features/<Feature>/...`: Feature-grouped services/DTOs/abstractions
- `HrSystem.Infrastructure/Repositories/<Feature>/...`: Feature-grouped repository implementations
- `HrSystem.Web/Features/<Feature>/...`: Feature-grouped MVC + API controllers and view-models
- `HrSystem.Web/Views/<ControllerName>/...`: MVC views (standard ASP.NET Core conventions)

## Features (Implemented)

- **Core HR / Master Data**: Department/Designation/EmploymentType/Shifts/Religion/BloodGroup CRUD.
- **Employees**: Employee CRUD + Bangladesh fields (NID/TIN/passport, addresses, bank/mobile banking) + document/file uploads.
- **Attendance**: Shift setup, punch endpoint (`POST /api/attendance/punch`), derived metrics (worked/late/early/missing punch), batch recompute (`POST /api/attendance/process`).
- **Leave (MVP)**: Leave types/policies, holiday/weekend calendars, requests with multi-level approval, balances, encashments.
- **Recruitment (MVP)**: Job postings, candidates, applications, interviews (CRUD).
- **Onboarding (MVP)**: Joining form, document checklist, orientation checklist, asset assignment, handbook + acknowledgement.
- **Offboarding (MVP)**: Exit interview, clearance items, final settlement.
- **Task & Workforce**: Employee task assignment, daily work logs (incl. WFH), productivity/team performance report.
- **Overtime (OT)**: Auto OT generation from attendance, holiday/double OT rules (policy multipliers), OT approval workflow.
- **Payroll Integration**: Period export/summary combining attendance + leave deductions + overtime + bonuses + salary adjustments.
- **Security (RBAC)**: Roles + permissions, permission-guarded endpoints (sample on Employees API), audit logs (request-level), activity tracking, login history.

## Prerequisites

- .NET SDK 8.x
- SQL Server (LocalDB is fine) OR Docker (for Compose)

## Configuration

- Connection string: `HrSystem.Web/appsettings.json` -> `ConnectionStrings:DefaultConnection`
- JWT: `HrSystem.Web/appsettings.json` -> `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`
- Seeded Super Admin user (Development): `HrSystem.Web/appsettings.json` -> `Admin:Username`, `Admin:Password`

Important: change `Jwt:Key` before using in real environments.

## Run (LocalDB / SQL Server)

1. Update `HrSystem.Web/appsettings.json` -> `Jwt:Key`
2. Run:
   - `dotnet run --project HrSystem.Web`
3. In Development, the database is created automatically via `EnsureCreated()` and seeded (master data + OT policy + RBAC roles/permissions + Super Admin user).

Note: `EnsureCreated()` does not update an existing database schema. If you pulled new code with model changes, drop/recreate your local database (or switch to EF Core migrations) before running.

Uploads are stored under `HrSystem.Web/wwwroot/uploads/` (git-ignored).

## Run (Docker Compose)

- `docker compose up --build`
- App: `http://localhost:8080`

## API Authentication (JWT)

1. Get token:
   - `POST /api/auth/token`
   - Body: `{ "username": "admin", "password": "admin123" }` (Development default)
2. Use:
   - `Authorization: Bearer <token>`

## Key API Endpoints

- Employees: `GET/POST /api/employees`, `GET/PUT/DELETE /api/employees/{id}`
- Attendance: `GET/POST /api/attendance`, `POST /api/attendance/punch`, `POST /api/attendance/process`
- Leave: `GET/POST /api/leave-types`, `GET/POST /api/leave-requests`, `POST /api/leave-requests/{id}/approve`, `POST /api/leave-requests/{id}/reject`, `GET/POST /api/holidays`, `GET/POST /api/leave-encashments`
- Recruitment: `GET/POST /api/job-postings`, `GET/POST /api/candidates`, `GET/POST /api/job-applications`, `GET/POST /api/interviews`
- Onboarding/Offboarding: `GET/POST /api/onboardings`, `GET/POST /api/offboardings`, `GET/POST /api/employee-assets`
- Payroll Integration: `GET /api/payroll-integration/period-summary`, `POST /api/payroll-integration/export`
- Task & Workforce: `GET/POST /api/employee-tasks`, `GET/POST /api/daily-work-logs`, `GET /api/workforce/reports/productivity`
- Overtime (OT): `GET/POST /api/overtime-requests`, `POST /api/overtime-requests/{id}/approve`, `POST /api/overtime-requests/{id}/reject`, `POST /api/overtime-requests/auto-generate`
- Security/RBAC: `GET/POST/PUT/DELETE /api/security/users`, `GET/POST/PUT/DELETE /api/security/roles`, `GET/POST/PUT/DELETE /api/security/permissions`, `GET /api/audit-logs`, `GET /api/login-history`
