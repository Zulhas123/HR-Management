# HR Management System (ASP.NET Core MVC)

This repo implements an HRMS (Bangladesh-focused) using:

- ASP.NET Core MVC (UI) + REST APIs
- Clean Architecture style layering: `Domain` / `Application` / `Infrastructure` / `Web`
- Repository pattern
- Entity Framework Core (Code First) with SQL Server
- JWT authentication for API endpoints

## Implemented (Phase 1 MVP)

Step 1 - Core HR - Employee Management:

- Master data CRUD: Departments, Designations, Employment Types
- Employee CRUD (auto-generates `EmployeeCode` as `EMP-000001`, ...)
- MVC screens + basic Bootstrap UI
- REST API endpoints for employee CRUD

Step 2 - Attendance (MVP):

- Shift CRUD
- Manual attendance CRUD (employee + date + in/out + optional shift)
- REST API endpoints for shift + attendance CRUD

Step 3 - Leave Management (MVP):

- Leave Types CRUD (Casual/Sick/etc. seeded in Development)
- Leave Requests CRUD
- Basic approve/reject workflow
- REST API endpoints for leave types + leave requests (+ approve/reject actions)

## Solution Structure

- `HrSystem.Domain`: Entities + repository abstractions
- `HrSystem.Application`: Use-case services (CRUD + employee code generation)
- `HrSystem.Infrastructure`: EF Core `DbContext` + repository implementations
- `HrSystem.Web`: MVC UI + API controllers + JWT auth configuration

## Run (LocalDB)

1. Configure secrets:
   - Update `HrSystem.Web/appsettings.json` -> `Jwt:Key` (use a long random string)
2. Start the web app:
   - `dotnet run --project HrSystem.Web`
3. The database is created automatically in Development via `EnsureCreated()`.

## API (JWT)

1. Get token:
   - `POST /api/auth/token`
   - Body: `{ "username": "admin", "password": "admin123" }`
2. Use the token:
   - `Authorization: Bearer <token>`
3. Employee endpoints:
   - `GET /api/employees`
   - `GET /api/employees/{id}`
   - `POST /api/employees`
   - `PUT /api/employees/{id}`
   - `DELETE /api/employees/{id}`

4. Attendance endpoints:
   - `GET /api/shifts`
   - `POST /api/shifts`
   - `GET /api/attendance`
   - `POST /api/attendance`

5. Leave endpoints:
   - `GET /api/leave-types`
   - `POST /api/leave-types`
   - `GET /api/leave-requests`
   - `POST /api/leave-requests`
   - `POST /api/leave-requests/{id}/approve`
   - `POST /api/leave-requests/{id}/reject`

## Next Features (Planned)

From the provided documentation, Phase 1 MVP continues with:

- Attendance
- Leave management
- Employee self-service (ESS)
- Reporting
