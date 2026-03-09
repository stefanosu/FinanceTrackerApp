# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

```bash
# Restore dependencies and build
dotnet build

# Run the API (development)
dotnet run
# Development URLs: http://localhost:5280, https://localhost:7280

# Run database migrations
dotnet ef database update

# Add a new migration
dotnet ef migrations add <MigrationName>

# Run with watch (auto-reload on changes)
dotnet watch run
```

## Configuration

Secrets are managed via .NET User Secrets (UserSecretsId: `finance-tracker-api-dev`):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<postgres-connection-string>"
dotnet user-secrets set "Jwt:SecretKey" "<min-32-char-secret>"
dotnet user-secrets set "Claude:ApiKey" "<anthropic-api-key>"
```

## Architecture

This is a .NET 8 Web API for a personal finance tracking application. The codebase follows a simplified layered architecture within a single project:

### Directory Structure

- **FinanceTracker.API/** - Controllers and middleware
  - Controllers inherit from `BaseController` (route: `api/[controller]`)
  - `AuthController` uses versioned route: `api/v1/[controller]`
- **FinanceTracker.Data/** - DbContext and data seeding
- **FinanaceTracker.Domain/** (note: typo in folder name) - Entity models
- **FinanceTracker.Domain/** - Custom exceptions (NotFoundException, ValidationException)
- **Services/** - Business logic, DTOs, validators, and service interfaces
- **Migrations/** - EF Core migrations

### Key Patterns

**Soft Delete**: All major entities (User, Expense, Account, Transaction) implement `ISoftDeletable`. Global query filters in `FinanceTrackerDbContext` automatically exclude deleted records. Use `.IgnoreQueryFilters()` when deleted records are needed.

**DTO Pattern**: Controllers accept and return DTOs (in `Services/Dto/`), not entities. Services handle mapping between entities and DTOs.

**Validation**: FluentValidation validators in `Services/Validators/` are auto-registered and run via `ValidationActionFilter`.

**Rate Limiting**: Four policies configured in Program.cs:
- `auth` - 5/min (login/register)
- `api` - 100/min (authenticated endpoints)
- `ai` - 10/min (budget assistant)
- `general` - 30/min sliding window

**Authentication**: JWT tokens stored in HTTP-only cookies. `CurrentUserService` extracts user ID from claims. Cross-origin cookies use `SameSite=None` + `Secure=true` for Vercel frontend.

### Service Registration

Services are registered in `Program.cs` with `AddScoped<IService, Service>()`. When adding new services, follow the existing interface + implementation pattern.

### Database

PostgreSQL via Npgsql. Connection string from `ConnectionStrings:DefaultConnection`. Migrations run automatically on startup via `context.Database.MigrateAsync()`.

### AI Integration

`BudgetAssistantService` calls the Claude API with user financial context. API key from `Claude:ApiKey` configuration.
