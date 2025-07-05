using Asp.Versioning;
using Package.Identity;
using Package.ObjectStorage;
using Package.Redis;
using Package.Serilog;
using Package.Sharing.Ffmpeg;
using Service.Storage.AppSettings;
using Service.Storage.Services;
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
services.AddObjectStorage();
services.AddTransient<IVideoProcessorService, VideoProcessorService>();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseCoreOpenApi("/storage/api/v{apiVersion:apiVersion}", apiVersionSetBuilder =>
{
    apiVersionSetBuilder.HasApiVersion(new ApiVersion(1));
    apiVersionSetBuilder.HasApiVersion(new ApiVersion(2));
});
app.MapGet("/", () => "Service.Storage");
await app.InitializeFFmpeg();
await app.RunAsync();