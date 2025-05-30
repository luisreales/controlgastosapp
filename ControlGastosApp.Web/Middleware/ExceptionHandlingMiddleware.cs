using System.Net;
using System.Text.Json;
using ControlGastosApp.Web.Exceptions;

namespace ControlGastosApp.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case AppException appException:
                    response.StatusCode = appException.StatusCode;
                    errorResponse.Message = appException.Message;
                    if (exception is ValidationException validationException)
                    {
                        errorResponse.Errors = validationException.Errors;
                    }
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "No tiene autorización para acceder a este recurso.";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = _env.IsDevelopment() 
                        ? exception.Message 
                        : "Ha ocurrido un error interno en el servidor.";
                    break;
            }

            _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string TraceId { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }
} 