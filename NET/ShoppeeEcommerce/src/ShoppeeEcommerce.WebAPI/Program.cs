using Asp.Versioning.ApiExplorer;
using ShoppeeEcommerce.Application;
using ShoppeeEcommerce.Infrastructure;
using ShoppeeEcommerce.Persistence;
using ShoppeeEcommerce.WebAPI.Configuration;
using ShoppeeEcommerce.WebAPI.Configuration.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure services' options
builder.Services.ConfigureServicesOptions(builder.Configuration);
// Configure services to the container.
builder.Services
    .AddApplicationServices()
    .AddPersistenceServices(builder.Configuration)
    .AddInfrastructureServices()
    .AddWebAPIServices();

var app = builder.Build();
app.UseCors(CORSServiceCollectionExtensions.AllowedAllOriginsPolicy);
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            $"Shoppee Ecommerce API {description.GroupName.ToUpperInvariant()}");
    }
});

// For now, disable HTTPS because of Docker setup complexity
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/api/health");
app.MapControllers();

// Use to seed data for database
// Disable if not necessary
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var logger = services.GetRequiredService<ILogger<Program>>();
//    try
//    {
//        logger.LogInformation("Begin data seeding");
//        await DataSeeder.SeedData(services);
//        logger.LogInformation("Data seeding completed");
//    }
//    catch (Exception ex)
//    {
//        logger.LogError(ex, "An error occurred while seeding the database.");
//    }
//}

app.Run();
