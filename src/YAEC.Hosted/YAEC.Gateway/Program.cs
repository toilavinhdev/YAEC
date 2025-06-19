using YAEC.Gateway.AppSettings;
using YAEC.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args).WithEnvironment<AppSettings>();

var services = builder.Services;
services.AddCoreCors();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.Run();