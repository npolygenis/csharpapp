
using System.Diagnostics;

namespace CSharpApp.Api.Middleware;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await _next(context);

        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        _logger.LogInformation("Request {Method} {Path} executed in {ElapsedMilliseconds}ms",
            context.Request.Method,
            context.Request.Path,
            elapsedMilliseconds);
    }
}
