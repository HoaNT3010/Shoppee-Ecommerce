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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/api/health");

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
