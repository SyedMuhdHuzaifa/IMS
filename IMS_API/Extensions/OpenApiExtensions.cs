using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IMS_API.Extensions
{
    public static class OpenApiExtensions
    {
        public static IServiceCollection AddImsOpenApi(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "IMS - Inventory Management System API",
                        Version = "v1",
                        Description = "Enterprise Inventory Management System API with Multi-Warehouse Stock Management, Sales & Purchase Orders, Transfers, and Role-Based Authorization."
                    };

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter your JWT token directly. Example: 'eyJhbGciOiJIUzI1NiIsIn...'"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = securityScheme;

                    var securityRequirement = new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                    };

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(securityRequirement);

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}
