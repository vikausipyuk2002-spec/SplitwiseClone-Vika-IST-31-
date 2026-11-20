using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SplitwiseClone.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService problems) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception ex, CancellationToken ct)
    {
        var (status, title) = ex switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Failure"),
            CustomValidationException => (StatusCodes.Status400BadRequest, "Validation Failure"),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ConflictException => (StatusCodes.Status409Conflict, "Business Conflict"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        http.Response.StatusCode = status;

        ProblemDetails body = ex switch
        {
            ValidationException ve => new HttpValidationProblemDetails(ve.Errors)
            {
                Status = status,
                Title = title,
                Detail = ex.Message
            },
            _ => new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? "An internal error occurred." : ex.Message
            }
        };

        body.Instance = http.Request.Path;

        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            Exception = ex,
            ProblemDetails = body
        });
    }
}