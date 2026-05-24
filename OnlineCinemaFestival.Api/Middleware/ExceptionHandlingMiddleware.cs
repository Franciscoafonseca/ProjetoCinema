using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OnlineCinemaFestival.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment
    )
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await EscreverErroAsync(context, ex);
        }
    }

    private async Task EscreverErroAsync(HttpContext context, Exception exception)
    {
        var (statusCode, titulo, detalhe, logarErro) = MapearErro(exception);

        if (logarErro)
            _logger.LogError(exception, "Erro inesperado ao processar {Path}.", context.Request.Path);
        else
            _logger.LogInformation(
                exception,
                "Pedido {Path} terminou com erro tratado {StatusCode}.",
                context.Request.Path,
                statusCode
            );

        if (context.Response.HasStarted)
            throw exception;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            Detail = detalhe,
            Instance = context.Request.Path,
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(problem);
    }

    private (int StatusCode, string Titulo, string Detalhe, bool LogarErro) MapearErro(
        Exception exception
    )
    {
        return exception switch
        {
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso nao encontrado.",
                exception.Message,
                false
            ),
            ValidationException or ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Pedido invalido.",
                exception.Message,
                false
            ),
            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                "Operacao nao autorizada.",
                exception.Message,
                false
            ),
            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Operacao invalida para o estado atual.",
                exception.Message,
                false
            ),
            NotSupportedException => (
                StatusCodes.Status501NotImplemented,
                "Operacao nao suportada.",
                exception.Message,
                false
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro inesperado.",
                _environment.IsDevelopment()
                    ? exception.Message
                    : "Ocorreu um erro inesperado ao processar o pedido.",
                true
            ),
        };
    }
}
