using System.Text.Json.Serialization;
using ControlGastosApp.Web.Data;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure SQL Server DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add logging for the connection string
builder.Logging.AddConsole(); // Ensure console logging is enabled if not already

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// Registrar servicios
builder.Services.AddScoped<SqlDataService>();
builder.Services.AddScoped<GastosService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Agregar el middleware de manejo de excepciones
app.UseExceptionHandling();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar el middleware de validación de administrador
app.UseAdminValidation();

// Agregar el middleware de validación de solicitudes
app.UseRequestValidation();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
