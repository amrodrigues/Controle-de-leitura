using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Api.Domain.Exceptions;

namespace ReadingTracker.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        if (exception is AppException appEx)
        {
            httpContext.Response.StatusCode = appEx.StatusCode;

            var problem = new ValidationProblemDetails
            {
                Status = appEx.StatusCode,
                Title = appEx.Message,
                Instance = httpContext.Request.Path
            };

            if (!string.IsNullOrWhiteSpace(appEx.Field))
                problem.Errors.Add(appEx.Field, [appEx.Message]);

            await httpContext.Response.WriteAsJsonAsync(problem, ct);
            return true;
        }

        logger.LogError(exception, "Erro inesperado na API");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno no servidor."
        }, ct);

        return true;
    }
}