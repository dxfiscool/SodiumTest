using Microsoft.AspNetCore.Http.Extensions;
using Roblox.Services.Logging;

namespace Roblox.Website.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context); // we should process the request first

            var request = $"[{context.Request.Method.ToUpper()}] - {context.Request.GetEncodedUrl()} - {context.Response.StatusCode}";
            Console.WriteLine(request);
        }
    }
}
