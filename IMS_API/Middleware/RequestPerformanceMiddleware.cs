using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IMS_API.Middleware
{
    
    public class RequestPerformanceMiddleware
    {
        private const int SlowRequestThresholdMs = 500;
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestPerformanceMiddleware> _logger;

        public RequestPerformanceMiddleware(RequestDelegate next, ILogger<RequestPerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.TryAdd("X-Response-Time-Ms", stopwatch.ElapsedMilliseconds.ToString());
                return Task.CompletedTask;
            });

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                var method = context.Request.Method;
                var path = context.Request.Path;
                var statusCode = context.Response.StatusCode;

                if (elapsedMs > SlowRequestThresholdMs)
                {
                    _logger.LogWarning("⚠️ Slow Request Detected: {Method} {Path} responded {StatusCode} in {ElapsedMs}ms (Threshold: {Threshold}ms)",
                        method, path, statusCode, elapsedMs, SlowRequestThresholdMs);
                }
                else
                {
                    _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                        method, path, statusCode, elapsedMs);
                }
            }
        }
    }
}
