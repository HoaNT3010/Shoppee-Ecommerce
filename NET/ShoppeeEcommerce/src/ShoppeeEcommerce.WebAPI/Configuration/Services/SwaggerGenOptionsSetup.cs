using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public class SwaggerGenOptionsSetup : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider _provider;
        public SwaggerGenOptionsSetup(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }
        public void Configure(SwaggerGenOptions options)
        {
            // Automatically generate Api version from [ApiVersion] attributes
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "Shoppee Ecommerce API",
                        Version = description.ApiVersion.ToString(),
                        Description = "API documentation for Shoppee Ecommerce system",
                        Contact = new OpenApiContact
                        {
                            Name = "Nguyen Thai Hoa",
                            Url = new Uri("https://github.com/HoaNT3010/Shoppee-Ecommerce"),
                            Email = "hoa41300@gmail.com",
                        }
                    });
            }
        }
    }
}
