using System.Text;
using System.Threading.RateLimiting;

using backend.Services;
using backend.Services.Interfaces;

using FinanceTrackerAPI.FinanceTracker.API.Filters;
using FinanceTrackerAPI.FinanceTracker.API.Middleware;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.Services;
using FinanceTrackerAPI.Services.Interfaces;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Register Swagger services before building the app
builder.Services.AddEndpointsApiExplorer();  // Add support for OpenAPI/Swagger
builder.Services.AddSwaggerGen();            // Register Swagger generator

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationActionFilter>();
})
.AddJsonOptions(options =>
{
    // Use camelCase for JSON property names (frontend expects "message" not "Message")
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddHttpContextAccessor();


// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute clock skew
    };

    // Support JWT from cookies (HttpOnly cookie approach)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check for token in cookie first, then fall back to header
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ============================================================================
// RATE LIMITING CONFIGURATION
// ============================================================================
// Why: Prevents brute-force attacks, credential stuffing, and API abuse
// How: Uses .NET 8's built-in RateLimiter with different policies per endpoint type
//
// Key Concepts:
// - PermitLimit: Max requests allowed in the window
// - Window: Time period for counting requests
// - QueueLimit: Requests to queue when limit hit (0 = reject immediately)
// - AutoReplenishment: Automatically reset counter after window expires
// ============================================================================
builder.Services.AddRateLimiter(options =>
{
    // What happens when rate limit is exceeded
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // POLICY 1: Strict limit for authentication endpoints (login, register)
    // Why stricter? These are prime targets for brute-force attacks
    // 5 attempts per minute per IP is enough for legitimate users, blocks attackers
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;                        // 5 requests max
        limiterOptions.Window = TimeSpan.FromMinutes(1);       // Per 1 minute
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;                         // No queuing - reject immediately
    });

    // POLICY 2: Standard limit for authenticated API endpoints
    // More generous since user is already authenticated
    // 100 requests per minute handles normal usage patterns
    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;                      // 100 requests max
        limiterOptions.Window = TimeSpan.FromMinutes(1);       // Per 1 minute
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2;                         // Queue up to 2 requests
    });

    // POLICY 3: Sliding window for general/public endpoints
    // Sliding window is smoother - prevents burst attacks at window boundaries
    options.AddSlidingWindowLimiter("general", limiterOptions =>
    {
        limiterOptions.PermitLimit = 30;                       // 30 requests max
        limiterOptions.Window = TimeSpan.FromMinutes(1);       // Per 1 minute
        limiterOptions.SegmentsPerWindow = 6;                  // 6 segments = 10-second granularity
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // POLICY 4: Strict limit for AI endpoints to control API costs
    // 10 requests per minute is reasonable for chat interactions
    options.AddFixedWindowLimiter("ai", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;                       // 10 requests max
        limiterOptions.Window = TimeSpan.FromMinutes(1);       // Per 1 minute
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;                         // No queuing - reject immediately
    });

    // Global fallback - applies when no specific policy is set
    // Uses client IP address as the partition key
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        // Use IP address as partition key (each IP gets its own limit)
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    // Custom response when rate limited
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/problem+json";

        // Include Retry-After header (tells client when to retry)
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
        }

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc6585#section-4",
            title = "Too Many Requests",
            status = 429,
            detail = "Rate limit exceeded. Please slow down your requests.",
            instance = context.HttpContext.Request.Path
        }, cancellationToken);
    };
});

// Configure CORS for cookie support - restricted to specific methods and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
              .WithHeaders("Content-Type", "Authorization", "X-Requested-With", "Accept")
              .AllowCredentials(); // Required for cookies
    });
});

// Register application services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IBudgetAssistantService, BudgetAssistantService>();
builder.Services.AddHttpClient();

// NOTE: HSTS and HTTPS redirection are NOT configured here because:
// - In production (Render), SSL termination happens at the proxy/load balancer
// - The proxy handles HSTS headers and HTTPS enforcement
// - Configuring them here would cause redirect loops behind a reverse proxy

//Adding Data to DB
builder.Services.AddDbContext<FinanceTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed data (development only - see DataSeeder for security considerations)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FinanceTrackerDbContext>();
    var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await context.Database.MigrateAsync();
    await DataSeeder.SeedData(context, environment, logger);
}

// Configure Kestrel URLs
// In development, we allow both HTTP and HTTPS for easier testing
// In production (Render, etc.), use HTTP only - the hosting platform handles SSL termination
if (app.Environment.IsDevelopment())
{
    app.Urls.Add("http://localhost:5280");  // HTTP - dev only
    app.Urls.Add("https://localhost:7280"); // HTTPS
}
else
{
    // Production: Use PORT env var (Render sets this) or default to 10000
    var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
    app.Urls.Add($"http://+:{port}");
}

// Enable Swagger middleware to serve the Swagger UI and API documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    // Enable Swagger to generate Swagger JSON
    app.UseSwaggerUI();  // Serve Swagger UI for API interaction
}

// ============================================================================
// HTTPS REDIRECTION & HSTS
// ============================================================================
// NOTE: When deployed behind a reverse proxy (Render, Heroku, etc.), the proxy
// handles SSL termination. The app receives HTTP requests from the proxy,
// so we should NOT use HTTPS redirection - it would cause redirect loops.
// HSTS headers are also handled by the proxy/CDN layer.
// ============================================================================
// Only enable HTTPS redirection in development with local HTTPS
if (app.Environment.IsDevelopment())
{
    // In dev, we can optionally redirect HTTP to HTTPS if testing locally
    // app.UseHttpsRedirection();
}

// Configure the HTTP request pipeline
app.UseRouting();

// CORS must run early so its response headers apply to ALL responses (including errors).
// If CORS runs after GlobalExceptionHandler, 401/500 responses bypass CORS and browser blocks them.
app.UseCors("AllowFrontend");

// Security headers - add early in pipeline so all responses get them
app.UseSecurityHeaders();

// Register global exception handling middleware (after routing, before controllers)
app.UseMiddleware<GlobalExceptionHandler>();

// Enable Rate Limiting (before auth - we want to rate limit even failed auth attempts)
app.UseRateLimiter();

// Enable Authentication and Authorization (CRITICAL: must be after CORS, before MapControllers)
app.UseAuthentication();
app.UseAuthorization();

// Map controllers to routes
app.MapControllers();

app.Run();
