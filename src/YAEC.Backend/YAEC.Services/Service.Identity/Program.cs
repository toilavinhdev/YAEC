using Package.Identity;
using Package.Logger;
using Package.MongoDb;
using Package.OpenApi;
using Package.Shared.Extensions;
using Package.Shared.Mediator;
using Service.Identity;

var builder = WebApplication.CreateBuilder(args).WithEnvironment<AppSettings>();
var services = builder.Services;
services.AddCoreLogger();
services.AddHttpContextAccessor();
services.AddCoreCors();
services.AddOpenApi(typeof(Program).Assembly);
services.AddCoreMediator(typeof(Program).Assembly);
services.AddCoreIdentity();
services.AddMongoDb();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseOpenApi();
app.MapGet("/", () => "Service.Identity");
app.Run();