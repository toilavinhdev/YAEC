using Package.FFmpeg;
using Package.Identity;
using Package.Logger;
using Package.OpenApi;
using Package.ObjectStorage;
using Package.Shared.Extensions;
using Service.Storage;
using Service.Storage.Services;

var builder = WebApplication.CreateBuilder(args).WithEnvironment<AppSettings>();
var services = builder.Services;
services.AddCoreLogger();
services.AddHttpContextAccessor();
services.AddCoreCors();
services.AddOpenApi(typeof(Program).Assembly);
services.AddCoreIdentity();
services.AddS3Manager();
services.AddScoped<IVideoProcessorService, VideoProcessorService>();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseOpenApi();
app.MapGet("/", () => "Service.Storage");
await app.InitializeFFmpeg();
app.Run();