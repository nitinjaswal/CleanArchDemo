# CleanArchDemo — Clean Architecture with CQRS & MediatR in .NET 9

A production-grade reference implementation of **Clean Architecture** built with **.NET 9**, demonstrating real-world patterns used in enterprise applications.

---

## What This Project Demonstrates

This project was built to deeply understand and practically implement architectural patterns commonly used in large-scale enterprise .NET applications.

### Architecture
The solution follows **Clean Architecture** principles with strict dependency rules:

```
API Layer              → References Application + Infrastructure (DI wiring only)
Application Layer      → References Domain only
Infrastructure Layer   → References Domain only
Domain Layer           → References nothing — pure C#, zero NuGet packages
```

### Patterns & Principles Implemented
- **Clean Architecture** — strict layer separation with inward dependency rule
- **CQRS** — Commands (write) and Queries (read) separated into distinct models and handlers
- **MediatR** — decouples Controllers from business logic via in-process messaging
- **Repository Pattern** — abstracts data access behind Domain interfaces
- **Unit of Work** — ensures multiple repository operations commit atomically in one transaction
- **Pipeline Behaviours** — cross-cutting concerns (logging, validation) applied automatically to every request without touching individual handlers
- **Dependency Inversion Principle** — high-level modules depend on abstractions, not concretions

---

## Tech Stack

| Technology | Purpose |
|---|---|
| .NET 9 | Framework |
| ASP.NET Core Web API | HTTP layer |
| Entity Framework Core | ORM |
| SQL Server | Database |
| MediatR 12 | CQRS in-process messaging |
| FluentValidation | Request validation |
| Swagger / Swashbuckle | API documentation |

---

## Solution Structure

```
CleanArchDemo/
├── Domain/                          ← Core — zero dependencies
│   ├── Entities/
│   │   └── User.cs                  ← Pure domain entity, private setters, factory method
│   └── Interfaces/
│       ├── IUserRepository.cs       ← Repository abstraction
│       └── IUnitOfWork.cs           ← Unit of Work abstraction
│
├── Application/                     ← Business logic — references Domain only
│   ├── Features/
│   │   └── Users/
│   │       ├── Commands/
│   │       │   └── CreateUser/
│   │       │       ├── CreateUserCommand.cs
│   │       │       └── CreateUserCommandHandler.cs
│   │       └── Queries/
│   │           └── GetUserById/
│   │               ├── GetUserByIdQuery.cs
│   │               └── GetUserByIdQueryHandler.cs
│   ├── Common/
│   │   └── Behaviours/
│   │       ├── LoggingBehaviour.cs      ← Logs request name + execution time
│   │       └── ValidationBehaviour.cs  ← Validates every command before Handler runs
│   └── DependencyInjection.cs          ← Registers MediatR + Behaviours + Validators
│
├── Infrastructure/                  ← Data access — references Domain only
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── UnitOfWork.cs            ← Wraps SaveChangesAsync
│   ├── Repositories/
│   │   └── UserRepository.cs        ← Implements IUserRepository
│   └── DependencyInjection.cs       ← Registers DbContext + Repositories + UnitOfWork
│
└── API/                             ← Entry point — references Application + Infrastructure
    ├── Controllers/
    │   └── UsersController.cs       ← Thin controller, only calls _mediator.Send()
    ├── Program.cs                   ← DI wiring + exception handling middleware
    └── appsettings.json
```

---

## Key Design Decisions

### Why Clean Architecture over N-Layer?
In traditional N-Layer architecture, the business logic layer directly references the data access layer — meaning business logic is coupled to EF Core, SQL Server, and repository implementations. In Clean Architecture, the **Domain layer defines interfaces** and the **Infrastructure layer implements them** — business logic has zero knowledge of how data is stored. This is enforced at compile time by project references, not just convention.

### Why CQRS?
Separating Commands (write) and Queries (read) into distinct models prevents a common problem in enterprise applications — the "God Service" that handles every operation and grows indefinitely. Each Handler has a single responsibility. Read models can be optimized independently of write models (e.g. direct DB projections bypassing domain entities for performance).

### Why Unit of Work?
Calling `SaveChangesAsync` inside each repository method creates separate database transactions per operation. If a Handler needs to save a User and an AuditLog atomically — both must succeed or both must fail. Unit of Work removes `SaveChangesAsync` from repositories and lets the Handler commit all changes in a single transaction at the end.

### Why MediatR Pipeline Behaviours?
Cross-cutting concerns like logging and validation should not be duplicated in every Handler. Pipeline Behaviours wrap every request automatically — `LoggingBehaviour` logs request name and execution time, `ValidationBehaviour` runs FluentValidation rules and throws before the Handler executes if validation fails. New concerns can be added globally without touching existing Handlers.

---

## Request Flow

```
HTTP POST /api/users
    ↓
UsersController         [API layer]         — calls _mediator.Send(command)
    ↓
LoggingBehaviour        [Application layer] — logs "Starting CreateUserCommand"
    ↓
ValidationBehaviour     [Application layer] — validates Name and Email
    ↓
CreateUserCommandHandler [Application layer] — business logic
    ↓
IUserRepository         [Domain interface]  — DI injects UserRepository at runtime
    ↓
UserRepository          [Infrastructure]    — EF Core tracks entity
    ↓
IUnitOfWork             [Domain interface]  — DI injects UnitOfWork at runtime
    ↓
UnitOfWork              [Infrastructure]    — SaveChangesAsync — one transaction
    ↓
SQL Server
    ↓
LoggingBehaviour        [Application layer] — logs "Completed in 45ms"
    ↓
HTTP 201 Created
```

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (local or express)
- Visual Studio 2022 or VS Code

### Setup

```bash
# Clone the repository
git clone https://github.com/nitinjaswal/CleanArchDemo.git
cd CleanArchDemo

# Update connection string in API/appsettings.Development.json
"DefaultConnection": "Server=localhost;Database=CleanArchDemo;Trusted_Connection=True;TrustServerCertificate=True"

# Run migrations
dotnet ef database update --project Infrastructure --startup-project API

# Run the application
dotnet run --project API
```

### Test via Swagger
Navigate to `https://localhost:{port}/swagger`

**Create a user:**
```json
POST /api/users
{
  "name": "Nitin Jaswal",
  "email": "nitinjas.chd@gmail.com"
}
```

**Get user by ID:**
```
GET /api/users/1
```

---

## About

Built by **Nitin Jaswal** — Senior Software Engineer with 12+ years of experience in Microsoft technologies including .NET 8/9, ASP.NET Core, Angular, Azure, and Microservices.

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Nitin%20Jaswal-blue)](https://www.linkedin.com/in/nitin-jaswal)
