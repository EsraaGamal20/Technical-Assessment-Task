# App API — Technical Assessment

A production-style **ASP.NET Core 9** REST API built with Clean Architecture, demonstrating JWT authentication, project & task management, Redis-backed OTP, and full Swagger documentation.

---

## Table of Contents

- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [API Overview](#api-overview)
- [Demo Credentials](#demo-credentials)
- [Project Structure](#project-structure)
- [Design Decisions](#design-decisions)

---

## Architecture

```
src/
├── App.Domain          # Entities, enums, domain interfaces (no dependencies)
├── App.Application     # Use-cases, DTOs, validators, service interfaces
├── App.Infrastructure  # JWT, BCrypt, Redis OTP, cross-cutting services
├── App.Persistence     # EF Core 9, SQL Server, repositories, Unit of Work
└── App.Api             # ASP.NET Core host, controllers, Swagger, middleware
```

**Dependency direction:** `Api → Application ← Infrastructure/Persistence → Domain`

---

## Prerequisites

| Tool | Version |
|---|---|
| .NET SDK | 9.0+ |
| SQL Server | 2019+ (or LocalDB / Docker) |
| Redis | 7+ (optional — OTP falls back gracefully) |

---

## Quick Start

```bash
# 1. Clone
git clone <repo-url>
cd "Technical Assessment Task"

# 2. Restore & build
dotnet build

# 3. (Optional) override connection strings
#    Copy appsettings.Development.json and edit, or use environment variables

# 4. Run (migrations + seed run automatically in Development)
dotnet run --project src/App.Api

# 5. Open Swagger UI
#    https://localhost:5001/swagger   (or http://localhost:5000/swagger)
```

> **No SQL Server?** The app starts without a database — migration/seed errors are caught and logged as warnings. The `GET /` health endpoint still responds.

---

## Configuration

### Connection Strings (`appsettings.Development.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=AppDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Connect Timeout=3"
  }
}
```

### JWT (`appsettings.json` or environment variables)

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR-SECRET-KEY-MIN-32-CHARS",
    "Issuer":    "AppApi",
    "Audience":  "AppClients",
    "ExpiryMinutes": 60
  }
}
```

### Redis (optional)

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### CORS (optional, defaults to localhost)

```json
{
  "CorsSettings": {
    "AllowedOrigins": ["https://yourfrontend.com"]
  }
}
```

---

## API Overview

All endpoints return a consistent envelope:

```json
{
  "succeeded": true,
  "message":   "...",
  "data":      { ... },
  "errors":    null
}
```

### Auth — `/api/auth`

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/auth/register/initiate` | ✗ | Register + send OTP |
| POST | `/api/auth/register/verify-otp` | ✗ | Verify OTP → JWT |
| POST | `/api/auth/register/resend-otp` | ✗ | Resend OTP |
| POST | `/api/auth/login` | ✗ | Login → JWT |

### Projects — `/api/projects`

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/projects` | ✓ | Create project |
| GET | `/api/projects` | ✓ | List own projects (paged) |
| GET | `/api/projects/{id}` | ✓ | Get project + tasks |
| PUT | `/api/projects/{id}` | ✓ | Update project |
| DELETE | `/api/projects/{id}` | ✓ | Soft-delete project |

### Tasks — `/api/projects/{projectId}/tasks`

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/projects/{projectId}/tasks` | ✓ | Create task |
| GET | `/api/projects/{projectId}/tasks` | ✓ | List tasks (paged) |
| GET | `/api/projects/{projectId}/tasks/{id}` | ✓ | Get task |
| PATCH | `/api/projects/{projectId}/tasks/{id}/status` | ✓ | Update status |
| DELETE | `/api/projects/{projectId}/tasks/{id}` | ✓ | Soft-delete task |

#### Task Status Transitions

```
Todo ──► InProgress ──► Done
 ▲           │            │
 │           ▼            ▼
 └───── Cancelled ◄── InProgress
```

| From | Allowed next states |
|------|---------------------|
| Todo | InProgress, Cancelled |
| InProgress | Done, Todo, Cancelled |
| Done | InProgress |
| Cancelled | Todo |

---

## Demo Credentials

After first run in Development mode the database is seeded automatically:

| Field | Value |
|-------|-------|
| Email | `demo@app.local` |
| Password | `Demo@1234` |

The seed creates **2 projects** and **4 tasks** owned by the demo user. The seed is idempotent — safe to run multiple times.

---

## Project Structure

```
src/
├── App.Domain/
│   ├── Common/          BaseEntity<TKey>
│   ├── Entities/        ApplicationUser, Project, TaskItem
│   └── Enums/           TaskItemStatus, TaskPriority
│
├── App.Application/
│   ├── Common/
│   │   ├── Exceptions/  AppException hierarchy
│   │   └── Models/      Result<T>, PagedResult<T>, PaginationRequest
│   ├── Features/
│   │   ├── Auth/        IAuthService, AuthService, DTOs, validators
│   │   ├── Projects/    IProjectService, ProjectService, DTOs, validators
│   │   └── Tasks/       ITaskService, TaskService, DTOs, validators
│   └── Interfaces/
│       ├── Persistence/ IRepository<T>, IUnitOfWork, specialised repos
│       └── Services/    IJwtTokenService, IPasswordHasher, IOtpService, IDateTimeProvider, ICurrentUserService
│
├── App.Infrastructure/
│   ├── Authentication/  JwtTokenService, AppClaimTypes
│   ├── Security/        BcryptPasswordHasher
│   ├── Otp/             RedisOtpService
│   ├── Services/        DateTimeProvider
│   └── Options/         JwtSettings, RedisSettings, CorsSettings
│
├── App.Persistence/
│   ├── Context/         AppDbContext, AppDbContextFactory
│   ├── Configurations/  EF entity type configs
│   ├── Interceptors/    AuditableEntityInterceptor (soft-delete, audit timestamps)
│   ├── Migrations/
│   ├── Repositories/    GenericRepository<T>, UserRepository, ProjectRepository, TaskRepository
│   ├── Seeding/         DatabaseSeeder
│   └── UnitOfWork/      UnitOfWork
│
└── App.Api/
    ├── Configuration/   SwaggerSetup, CorsSetup
    ├── Controllers/     AuthController, ProjectsController, TasksController
    ├── Filters/         ValidationFilter, EnumSchemaFilter, DefaultResponsesOperationFilter
    ├── Middleware/       AppExceptionHandler, UnhandledExceptionHandler
    ├── Services/        CurrentUserService
    ├── GlobalUsings.cs
    └── Program.cs
```

---

## Design Decisions

**Clean Architecture** — domain has zero external dependencies; application layer defines interfaces that outer layers implement (DIP).

**Result pattern** — every service method returns `Result<T>` instead of throwing, keeping controller code declarative and the happy-path obvious.

**Soft delete** — `IsDeleted` flag + global EF query filters ensure deleted records are hidden from all queries by default without explicit `WHERE` clauses.

**Idempotent seeder** — checks for the demo user before inserting; safe to call on every startup.

**JWT with short clock skew** — `ClockSkew = 30s` to minimise the window during which a just-expired token is still accepted.

**Redis OTP** — OTP codes are stored in Redis with a TTL; if Redis is unavailable the app degrades gracefully (OTP operations will fail, auth/project/task routes continue to work).

**Swashbuckle 10 + Microsoft.OpenApi 2.x** — enums rendered as strings, XML doc comments surfaced, JWT Bearer security requirement wired via the new `OpenApiSecuritySchemeReference` API.
