using System.Net;
using System.Text.Json;
using ShiftDock.Application.DTOs.Common;
using ShiftDock.Domain.Exceptions;

namespace ShiftDock.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorResponse = new ErrorResponse
        {
            Code = "INTERNAL_SERVER_ERROR",
            Message = exception.Message
        };

        // Handle specific exception types
        if (exception is UnauthorizedAccessException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            errorResponse.Code = "UNAUTHORIZED";
        }
        else if (exception is UserNotFoundException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            errorResponse.Code = "USER_NOT_FOUND";
        }
        else if (exception is ArgumentException || exception is InvalidOperationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            errorResponse.Code = "BAD_REQUEST";
        }

        var response = ApiResponse<object>.FailureResponse(
            "An error occurred while processing your request.",
            errorResponse
        );

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
