using System.Text.Json.Serialization;
using ControlGastosApp.Web.Data;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios
builder.Services.AddSingleton<IJsonDataService>(provider => 
    new JsonDataService(Path.Combine(builder.Environment.ContentRootPath, "Data", "data.json")));

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
