using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ControlGastosApp.Web.Exceptions;

namespace ControlGastosApp.Web.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(
            RequestDelegate next,
            ILogger<ValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Solo validar métodos POST y PUT
            if (context.Request.Method != "POST" && context.Request.Method != "PUT")
            {
                await _next(context);
                return;
            }

            // Verificar si el cuerpo está vacío
            if (!context.Request.HasJsonContentType())
            {
                await HandleValidationError(context, "El contenido debe ser JSON");
                return;
            }

            try
            {
                // Leer el cuerpo de la solicitud
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(requestBody))
                {
                    await HandleValidationError(context, "El cuerpo de la solicitud no puede estar vacío");
                    return;
                }

                // Parsear el JSON
                var jsonNode = JsonNode.Parse(requestBody);
                if (jsonNode == null)
                {
                    await HandleValidationError(context, "JSON inválido");
                    return;
                }

                // Validar el contenido
                var validationErrors = ValidateJsonNode(jsonNode);
                if (validationErrors.Any())
                {
                    await HandleValidationErrors(context, validationErrors);
                    return;
                }

                // Restaurar el cuerpo de la solicitud para que pueda ser leído por los controladores
                var requestData = System.Text.Encoding.UTF8.GetBytes(requestBody);
                context.Request.Body = new MemoryStream(requestData);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar la solicitud");
                await HandleValidationError(context, "Error al procesar la solicitud");
            }
        }

        private List<string> ValidateJsonNode(JsonNode node, string path = "")
        {
            var errors = new List<string>();

            if (node is JsonObject obj)
            {
                foreach (var property in obj)
                {
                    var currentPath = string.IsNullOrEmpty(path) ? property.Key : $"{path}.{property.Key}";
                    errors.AddRange(ValidateJsonNode(property.Value, currentPath));
                }
            }
            else if (node is JsonArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    var currentPath = $"{path}[{i}]";
                    errors.AddRange(ValidateJsonNode(array[i], currentPath));
                }
            }
            else if (node is JsonValue value)
            {
                if (value.TryGetValue(out string stringValue))
                {
                    if (string.IsNullOrWhiteSpace(stringValue))
                    {
                        errors.Add($"El campo '{path}' no puede estar vacío");
                    }
                }
                else if (value.TryGetValue(out decimal decimalValue))
                {
                    if (decimalValue < 0)
                    {
                        errors.Add($"El campo '{path}' no puede ser negativo");
                    }
                }
                else if (value.TryGetValue(out int intValue))
                {
                    if (intValue < 0)
                    {
                        errors.Add($"El campo '{path}' no puede ser negativo");
                    }
                }
            }

            return errors;
        }

        private async Task HandleValidationError(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

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

        private async Task HandleValidationErrors(HttpContext context, List<string> errors)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorResponse = new ErrorResponse
            {
                Message = "Se encontraron errores de validación",
                TraceId = context.TraceIdentifier,
                Errors = new Dictionary<string, string[]>
                {
                    { "ValidationErrors", errors.ToArray() }
                }
            };

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(result);
        }
    }
} 