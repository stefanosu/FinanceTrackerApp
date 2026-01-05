# Finance Tracker Development Log

## Purpose
Track how I'm using AI tools (Claude Code + Cursor) to build features, what I'm learning, and demonstrate strategic AI usage to employers.

## Log Format

### [Feature Name] - [Date]

**What I Built:**
- Brief description of the feature
- Technical components involved

**AI Tools Used:**
- Which tool (Claude Code vs Cursor vs Manual)
- Specific prompts or approach
- Why I chose this tool for this task

**What I Learned:**
- Technical concepts or patterns
- Gaps AI filled vs. what I had to understand deeply
- Decisions I made differently than AI suggested

**Interview Talking Points:**
- How this demonstrates my skills
- Trade-offs I considered
- Scale/performance implications

---

## Entries

### Codebase Architecture Analysis - 2026-01-04

**What I Built:**
- Comprehensive architectural documentation and understanding
- Mapped out entire tech stack (.NET 8 backend, Next.js 15 frontend, PostgreSQL)
- Identified all major components: Auth system, User management, Expense tracking, Account management, Transaction system
- Documented 25+ API endpoints across 6 controllers
- Analyzed layered architecture pattern (API → Services → Data → Domain)

**AI Tools Used:**
- **Tool**: Claude Code CLI with Explore agent
- **Prompt**: "Give me an architectural overview of this finance tracker app. What's the tech stack, what's already built, and what are the major components?"
- **Why This Tool**:
  - Claude Code's Explore agent is specifically designed for codebase analysis
  - Can read multiple files in parallel and synthesize patterns
  - Better than manual file-by-file exploration for large codebases
  - Faster than Cursor for pure reconnaissance without editing

**What I Learned:**
- **Architecture Pattern Recognition**: The codebase follows clean architecture with clear layer separation - API controllers delegate to services, services use repositories/DbContext, domain entities are separate
- **Security Implementation**: JWT tokens stored in HTTP-only cookies (more secure than localStorage), BCrypt for password hashing with auto-salting
- **EF Core Conventions**: Automatic migration on startup, data seeding patterns, DbContext lifecycle management in ASP.NET Core DI
- **Next.js 15 App Router**: Server/client component patterns, layout composition, TanStack Query for server state
- **FluentValidation Integration**: Both service-level validation and action filter validation for controller parameters
- **Testing Strategy**: Unit tests with Moq for mocking services, in-memory EF Core for integration tests

**Gaps AI Filled vs. Deep Understanding:**
- **AI Filled**: Quickly mapped file structure, identified all endpoints, found configuration patterns
- **I Had to Understand**:
  - Why JWT tokens are in cookies vs. headers (XSS vs CSRF trade-offs)
  - Why layered architecture matters (testability, maintainability, dependency direction)
  - EF Core migration strategy (when to run migrations, seeding data patterns)
  - The relationship between DTOs and domain entities (separation of concerns)

**Decisions I Made Differently:**
- N/A - This was pure reconnaissance, no implementation decisions yet
- Did note that some security configs (JWT secret) are hardcoded in appsettings.json - should be environment variables in production

**Interview Talking Points:**
- **Strategic Tool Use**: "I use Claude Code for rapid codebase exploration before diving into implementation. Rather than spending hours manually reading files, I use AI to build a mental model quickly, then verify critical paths myself."
- **Security Awareness**: "I noticed the app uses HTTP-only cookies for JWT storage. This prevents XSS attacks since JavaScript can't access the tokens, though it requires CSRF protection. The trade-off is appropriate for a web application."
- **Architecture Comprehension**: "The layered architecture with dependency injection makes this highly testable. Controllers depend on IAuthService interface, not concrete implementation, so unit tests can mock the service layer completely."
- **Scale Implications**: "Current architecture has all services as scoped dependencies hitting DbContext directly. For scale, I'd consider: (1) Caching layer for category/subcategory lookups, (2) Read replicas for reporting queries, (3) Rate limiting on expensive endpoints like expense list with filters."
- **Tech Stack Modern Practices**: ".NET 8 with EF Core 9, Next.js 15 App Router, React Query - these are current industry standards. Shows I can work with modern tooling, not legacy stacks."
- **CI/CD Pipeline**: "GitHub Actions pipeline runs tests, security scanning, and code formatting checks. Demonstrates understanding of DevOps practices and code quality automation."

**Next Steps Identified:**
- Goal tracking feature appears partially implemented (frontend hooks exist but backend may be incomplete)
- Could add expense analytics/reporting endpoints
- Could implement budget limits and alerts
- Could add recurring expense tracking
- Consider adding comprehensive integration tests beyond unit tests
