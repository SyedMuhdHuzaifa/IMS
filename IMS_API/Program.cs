using Data;
using IMS_API.Extensions;
using IMS_API.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers with JSON formatting
builder.Services.AddControllers();

// 2. Configure OpenAPI with JWT Bearer Authentication and API Documentation
builder.Services.AddImsOpenApi();

// 3. Register all IMS repositories and business services
builder.Services.AddImsServices(builder.Configuration);

// 4. Configure JWT Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// 5. Configure CORS (allow clients to connect)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- HTTP Pipeline ---

// A. Global Exception Handler (first in pipeline to catch all errors)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// B. Interactive API Exploration: Scalar UI and Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    // Generates /openapi/v1.json specification
    app.MapOpenApi();

    // Modern Scalar UI at /scalar/v1
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("IMS - Inventory Management API (Scalar)")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    // Classic Swagger UI at /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "IMS API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "IMS - Swagger UI";
    });

    // Root URL redirect straight to Scalar API reference
    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
}

app.UseHttpsRedirection();

// C. CORS
app.UseCors("AllowAll");

// D. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// E. Map Controller endpoints
app.MapControllers();

// F. Initialize and seed default database values (Roles, Users, Warehouses, Products, Stocks)
await DbInitializer.SeedAsync(app.Services);

app.Run();
