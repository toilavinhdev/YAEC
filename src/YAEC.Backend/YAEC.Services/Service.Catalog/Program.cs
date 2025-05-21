using Package.Identity;
using Package.Logger;
using Package.OpenApi;
using Package.RabbitMQ;
using Package.Redis;
using Package.Shared.Extensions;
using Package.Shared.Mediator;
using Service.Catalog;
using Service.Catalog.Events;

var builder = WebApplication.CreateBuilder(args).WithEnvironment<AppSettings>();
var services = builder.Services;
services.AddCoreLogger();
services.AddHttpContextAccessor();
services.AddCoreCors();
services.AddOpenApi(typeof(Program).Assembly);
services.AddCoreMediator(typeof(Program).Assembly);
services.AddCoreIdentity();
services.AddRedis();
services.AddRabbitMQ();
services.AddHostedService<TestConsumerService>();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseOpenApi();
app.MapGet("/", () => "Service.Catalog");
app.Run();