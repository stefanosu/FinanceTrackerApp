using backend.Services;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.Services;
using FinanceTrackerAPI.Services.Interfaces;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register Swagger services before building the app
builder.Services.AddEndpointsApiExplorer();  // Add support for OpenAPI/Swagger
builder.Services.AddSwaggerGen();            // Register Swagger generator

// Add services to the container.
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Configure CORS for cookie support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Required for cookies
    });
});

// Register application services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();

//Adding Data to DB
builder.Services.AddDbContext<FinanceTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Global exception handling is now handled by ExceptionHandlingFilter

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FinanceTrackerDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedData(context);
}

// Configure Kestrel to listen on both HTTP and HTTPS ports
app.Urls.Add("http://localhost:5280");  // HTTP
app.Urls.Add("https://localhost:7280"); // HTTPS

// Enable Swagger middleware to serve the Swagger UI and API documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    // Enable Swagger to generate Swagger JSON
    app.UseSwaggerUI();  // Serve Swagger UI for API interaction
}

// Configure the HTTP request pipeline
app.UseRouting();

// Enable CORS
app.UseCors("AllowFrontend");

// Map controllers to routes
app.MapControllers();

app.Run();
