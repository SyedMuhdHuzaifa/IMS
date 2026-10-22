using Data;
using IMS_API.Extensions;
using IMS_API.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers with JSON formatting
builder.Services.AddControllers();

// 2. Configure OpenAPI
builder.Services.AddOpenApi();

// 3. Register all IMS repositories and business services
builder.Services.AddImsServices(builder.Configuration);

// 4. Configure JWT Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// 5. Configure CORS (allow web clients to connect)
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

// B. OpenAPI & Interactive Scalar UI in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// C. CORS
app.UseCors("AllowAll");

// D. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// E. Map Controller endpoints
app.MapControllers();

// F. Initialize and seed default database values (Roles, Admin, Warehouse, Categories)
await DbInitializer.SeedAsync(app.Services);

app.Run();
