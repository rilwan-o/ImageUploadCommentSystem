using Azure.Storage.Blobs;
using Infrastructure.Contracts;
using Infrastructure.Models;
using Infrastructure.Service;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddSingleton(x =>
            {
                var configuration = x.GetRequiredService<IConfiguration>();
                var connectionString = configuration
                                       .GetConnectionString("AzureBlobStorageConnectionString");
                return new BlobServiceClient(connectionString);
            });
            services.Configure<AzureBlobStorage>(configuration.GetSection("AzureBlobStorage"));
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ILoggerService, LoggerService>();

            return services;
        }
    }
}
