using Asp.Versioning;
using Package.Elasticsearch;
using Package.Identity;
using Package.Redis;
using Package.Serilog;
using Service.Catalog.AppSettings;
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
services.AddRedis();
services.AddSingleNodeElasticsearch();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseCoreOpenApi("/catalog/api/v{apiVersion:apiVersion}", apiVersionSetBuilder =>
{
    apiVersionSetBuilder.HasApiVersion(new ApiVersion(1));
    apiVersionSetBuilder.HasApiVersion(new ApiVersion(2));
});
app.MapGet("/", () => "Service.Catalog");
app.Run();