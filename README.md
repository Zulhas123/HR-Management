# HR Management System (ASP.NET Core MVC)

Bangladesh-focused HRMS implemented with a minimal, maintainable structure:

- ASP.NET Core MVC (UI) + REST APIs
- Clean Architecture style layering: `Domain` / `Application` / `Infrastructure` / `Web`
- Repository pattern + dependency injection
- Entity Framework Core (Code First) with SQL Server
- JWT authentication for API endpoints
- Docker Compose for local SQL Server + app

## Architecture

- `HrSystem.Domain`: Entities + repository abstractions (no EF/UI concerns)
- `HrSystem.Application`: Use-case services (business logic, async)
- `HrSystem.Infrastructure`: EF Core `DbContext` + repository implementations
- `HrSystem.Web`: MVC UI + API controllers + auth/configuration

## Features (Implemented)

Core HR - Employee Management:

- Master data: Departments, Designations, Employment Types (MVC CRUD)
- Employees: create/edit/delete + auto-generated employee code (`EMP-000001`, ...)
- Joining/resignation fields (join date + resignation date)
- Digital employee file: photo upload, signature upload, document uploads, education, experience
- Transfer history + promotion history (per employee)
- Emergency contacts + family members (per employee)
- REST API: Employee CRUD (JWT protected)

Attendance (MVP):

- Shifts (MVC CRUD)
- Manual attendance records (employee + date + check-in/out + optional shift)
- REST API: Shifts + Attendance CRUD (JWT protected)

Leave Management (MVP):

- Leave Types (MVC CRUD; common types seeded in Development)
- Leave Requests (MVC CRUD) + basic approve/reject flow
- REST API: Leave Types + Leave Requests CRUD (+ approve/reject) (JWT protected)

Recruitment & Hiring (MVP):

- Job postings, candidates, applications (pipeline stage), interviews (MVC CRUD)
- REST API: Job postings, candidates, applications, interviews CRUD (JWT protected)

## Prerequisites

- .NET SDK 8.x
- SQL Server (LocalDB is fine) OR Docker (for Compose)

## Configuration

- Connection string: `HrSystem.Web/appsettings.json` -> `ConnectionStrings:DefaultConnection`
- JWT settings: `HrSystem.Web/appsettings.json` -> `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`
- Admin user for token (MVP): `HrSystem.Web/appsettings.json` -> `Admin:Username`, `Admin:Password`

Important: change `Jwt:Key` before using in real environments.

## Run (LocalDB / SQL Server)

1. Update `HrSystem.Web/appsettings.json` -> `Jwt:Key`
2. Start the app:
   - `dotnet run --project HrSystem.Web`
3. In Development, the database is created automatically via `EnsureCreated()` and seeded with basic master data.

Uploaded files (photos/signatures/documents) are stored under `HrSystem.Web/wwwroot/uploads/` and are ignored by git (`.gitignore`).

## Run (Docker Compose)

- Start SQL Server + web app:
  - `docker compose up --build`
- App:
  - `http://localhost:8080`

## API (JWT)

1. Get token:
   - `POST /api/auth/token`
   - Body: `{ "username": "admin", "password": "admin123" }`
2. Use the token header:
   - `Authorization: Bearer <token>`

Endpoints:

- Employees: `GET/POST /api/employees`, `GET/PUT/DELETE /api/employees/{id}`
- Attendance: `GET/POST /api/attendance`, `GET/POST /api/shifts`
- Leave: `GET/POST /api/leave-types`, `GET/POST /api/leave-requests`, `POST /api/leave-requests/{id}/approve`, `POST /api/leave-requests/{id}/reject`
- Recruitment: `GET/POST /api/job-postings`, `GET/POST /api/candidates`, `GET/POST /api/job-applications`, `GET/POST /api/interviews`

## Next Features (Planned)

From the provided documentation, the next modules include:

- Employee self-service (ESS)
- Reporting
- Onboarding/offboarding, performance, payroll integration, and more

