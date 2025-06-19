using Package.Identity;
using Package.MongoDb;
using Package.Serilog;
using Service.Identity.AppSettings;
using YAEC.Shared.Extensions;
using YAEC.Shared.Mediator;
using YAEC.Shared.OpenApi;

var builder = WebApplication.CreateBuilder(args).WithEnvironment<AppSettings>();

var services = builder.Services;
services.AddHttpContextAccessor();
services.AddCoreSerilog();
services.AddCoreCors();
services.AddCoreOpenApi(typeof(Program).Assembly);
services.AddCoreMediator(typeof(Program).Assembly);
services.AddCoreIdentity();
services.AddMongoDb();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseCoreOpenApi();
app.Run();