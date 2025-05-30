using Microsoft.AspNetCore.Builder;

namespace ControlGastosApp.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationMiddleware>();
        }

        public static IApplicationBuilder UseAdminValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminValidationMiddleware>();
        }

        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static IEndpointConventionBuilder MapStaticAssets(this IEndpointConventionBuilder builder)
        {
            return builder;
        }

        public static IApplicationBuilder MapStaticAssets(this IApplicationBuilder builder)
        {
            return builder;
        }
    }
} 