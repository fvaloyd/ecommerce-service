using System.Reflection;
using Ecommerce.Api.Startup;
using Ecommerce.Application;
using Ecommerce.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
    
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();

var app = builder.Build();

app.ConfigureSwagger(); 

await app.SeedDataHandle();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ConfigureKeys.SetupApiKeys(builder);

app.Run();