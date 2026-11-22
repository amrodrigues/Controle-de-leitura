namespace ReadingTracker.Api.Domain.Exceptions;

public abstract class AppException(string message, int statusCode, string? field = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string? Field { get; } = field;
}

public class ValidationRuleException(string message, string? field = null)
    : AppException(message, StatusCodes.Status400BadRequest, field);

public class UnauthorizedAppException(string message)
    : AppException(message, StatusCodes.Status401Unauthorized);

public class NotFoundAppException(string message)
    : AppException(message, StatusCodes.Status404NotFound);

public class ConflictAppException(string message, string? field = null)
    : AppException(message, StatusCodes.Status409Conflict, field);