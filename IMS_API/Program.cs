using Data;
using IMS_API.Extensions;
using IMS_API.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddImsOpenApi();

builder.Services.AddImsServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

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


app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestPerformanceMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("IMS - Inventory Management API (Scalar)")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "IMS API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "IMS - Swagger UI";
    });

    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
    app.MapGet("/swagger/v1/swagger.json", () => Results.Redirect("/openapi/v1.json"));
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await DbInitializer.SeedAsync(app.Services);

app.Run();
