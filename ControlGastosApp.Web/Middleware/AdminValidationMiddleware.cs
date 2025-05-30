using System.Net;
using System.Text.Json;
using ControlGastosApp.Web.Exceptions;

namespace ControlGastosApp.Web.Middleware
{
    public class AdminValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminValidationMiddleware> _logger;

        public AdminValidationMiddleware(
            RequestDelegate next,
            ILogger<AdminValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Por ahora, permitimos todas las solicitudes sin validación
            await _next(context);

            // TODO: Implementar validación de administrador cuando se agregue el sistema de autenticación
            /*
            // Verificar si el usuario está autenticado
            if (!context.User.Identity.IsAuthenticated)
            {
                await HandleUnauthorizedResponse(context, "Usuario no autenticado");
                return;
            }

            // Verificar si el usuario es administrador
            if (!context.User.IsInRole("Admin"))
            {
                await HandleUnauthorizedResponse(context, "No tiene permisos de administrador");
                return;
            }
            */
        }

        private async Task HandleUnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            var errorResponse = new ErrorResponse
            {
                Message = message,
                TraceId = context.TraceIdentifier
            };

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(result);
        }
    }
} 