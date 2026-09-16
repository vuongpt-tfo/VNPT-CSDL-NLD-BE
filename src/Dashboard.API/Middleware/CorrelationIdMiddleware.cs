using Serilog.Context;

namespace Dashboard.API.Middleware;

/// <summary>
/// Doc Correlation ID tu request header (hoac tu tao moi neu chua co),
/// dinh kem vao response header va Serilog LogContext - tat ca log
/// phat sinh trong 1 request deu mang cung 1 CorrelationId, phuc vu
/// truy vet request xuyen suot pipeline (NFR-HT-06 tracing/correlation).
/// </summary>
public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var existing)
            && !string.IsNullOrWhiteSpace(existing))
        {
            return existing.ToString();
        }

        return Guid.NewGuid().ToString("n");
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(
        this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationIdMiddleware>();
}
