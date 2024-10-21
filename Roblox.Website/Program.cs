using Microsoft.AspNetCore.HttpLogging;
using Roblox.Services;
using Roblox.Services.Logging;
using Roblox.Website.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<LoggingMiddleware>();

// setup signatures
SignatureController.Start();

Log.RobloxLog("Started Roblox.Website");

app.Run();

