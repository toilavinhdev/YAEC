using Package.Identity;
using Package.Logger;
using Package.OpenApi;
using Package.Redis;
using Package.Shared.Extensions;
using Package.Shared.Mediator;
using Service.Catalog;

var builder = WebApplication.CreateBuilder(args).WithEnvironment<AppSettings>();
var services = builder.Services;
services.AddCoreLogger();
services.AddHttpContextAccessor();
services.AddCoreCors();
services.AddOpenApi(typeof(Program).Assembly);
services.AddCoreMediator(typeof(Program).Assembly);
services.AddCoreIdentity();
services.AddRedis();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseOpenApi();
app.MapGet("/", () => "Service.Catalog");
app.Run();