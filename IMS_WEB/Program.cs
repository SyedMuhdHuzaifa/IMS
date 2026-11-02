using IMS_WEB.Handlers;
using IMS_WEB.Services;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. HttpContext Accessor & Handler
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthenticationHeaderHandler>();

// 2. Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// 3. API Base URL
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? "https://localhost:7206/api/";

if (!apiBaseUrl.EndsWith("/"))
{
    apiBaseUrl += "/";
}

// 4. API Clients
builder.Services.AddHttpClient<IAuthApiServices, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IProductApiServices, ProductApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<ISupplierApiService, SupplierApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<ICategoryApiService, CategoryApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<ICustomerApiService, CustomerApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IWarehouseApiService, WarehouseApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<ISalesOrderApiService, SalesOrderApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IPurchaseOrderApiService, PurchaseOrderApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();