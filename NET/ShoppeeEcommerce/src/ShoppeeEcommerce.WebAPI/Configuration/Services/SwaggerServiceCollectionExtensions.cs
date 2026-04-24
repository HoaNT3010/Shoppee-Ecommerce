using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerDocs(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                // Prefer explicit [SwaggerOperation(Tags = ...)] tags.
                // Fall back to controller name when no explicit tags are present.
                c.TagActionsBy(api =>
                {
                    // 1) If an explicit SwaggerOperationAttribute provides tags, use them.
                    var explicitTags = api.ActionDescriptor?.EndpointMetadata?
                        .OfType<SwaggerOperationAttribute>()
                        .SelectMany(a => a.Tags ?? Array.Empty<string>())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToArray();

                    if (explicitTags != null && explicitTags.Length > 0)
                        return explicitTags;

                    // 2) Fallback to controller route value (typical for controller-based endpoints)
                    if (api.ActionDescriptor?.RouteValues != null &&
                        api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName) &&
                        !string.IsNullOrWhiteSpace(controllerName))
                    {
                        return new[] { controllerName };
                    }

                    // 3) Final fallback: group by HTTP method + path so tag list isn't empty
                    return new[] { $"{api.HttpMethod ?? "UNKN"} {api.RelativePath}" ?? "Default" };
                });
            });

            services.ConfigureOptions<SwaggerGenOptionsSetup>();

            return services;
        }
    }
}
