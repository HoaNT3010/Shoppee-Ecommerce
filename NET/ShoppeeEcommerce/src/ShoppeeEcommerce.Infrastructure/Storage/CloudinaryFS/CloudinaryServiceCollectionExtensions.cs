using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Application.Abstractions.Storage;

namespace ShoppeeEcommerce.Infrastructure.Storage.CloudinaryFS
{
    public static class CloudinaryServiceCollectionExtensions
    {
        public static IServiceCollection AddCloudinary(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var cloudinarySettings = configuration
                .GetSection(CloudinaryOptions.SectionName)
                .Get<CloudinaryOptions>();
            services.AddSingleton(new Cloudinary(
                new Account(
                    cloudinarySettings!.CloudName,
                    cloudinarySettings.ApiKey,
                    cloudinarySettings.ApiSecret)));
            services.AddScoped<IFileService, CloudinaryFileService>();

            return services;
        }
    }
}
