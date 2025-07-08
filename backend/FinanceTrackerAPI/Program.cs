using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register Swagger services before building the app
builder.Services.AddEndpointsApiExplorer();  // Add support for OpenAPI/Swagger
builder.Services.AddSwaggerGen();            // Register Swagger generator

// Add services to the container.
builder.Services.AddControllers();

//Adding Data to DB
builder.Services.AddDbContext<FinanceTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Add global exception handler middleware
app.UseMiddleware<GlobalExceptionHandler>();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FinanceTrackerDbContext>();
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

// Map controllers to routes
app.MapControllers();

app.Run();
