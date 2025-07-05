using Asp.Versioning;
using Package.Hangfire;
using Package.Hangfire.Abstractions;
using Package.Identity;
using Package.Serilog;
using Service.Schedule.AppSettings;
using Service.Schedule.Services;
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
services.AddCoreHangfire();
services.AddScoped<IHangfireScheduleService, HelloWorldScheduleService>();

var app = builder.Build();
app.UseCoreExceptionHandler();
app.UseCors(CorsExtensions.AllowAll);
app.UseCoreIdentity();
app.UseCoreOpenApi("/schedule/api/v{apiVersion:apiVersion}", apiVersionSetBuilder =>
{
    apiVersionSetBuilder.HasApiVersion(new ApiVersion(1));
    apiVersionSetBuilder.HasApiVersion(new ApiVersion(2));
});
app.UseCoreHangfireScheduleJobs();
app.UseCoreHangfireDashboard("/hangfire");
app.MapGet("/", () => "Service.Schedule");
app.Run();