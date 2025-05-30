namespace ControlGastosApp.Web.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(string message, int statusCode = 400) 
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) 
            : base(message, 404)
        {
        }
    }

    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors) 
            : base("Se encontraron errores de validación", 400)
        {
            Errors = errors;
        }
    }

    public class BusinessException : AppException
    {
        public BusinessException(string message) 
            : base(message, 400)
        {
        }
    }
} 