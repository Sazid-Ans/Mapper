using System.Runtime.CompilerServices;

namespace DataTypeMapping.Utilities.AppSettingsDO
{
    public static class ServiceCollectionExt
    {
        // Extension method (clean name & PascalCase)
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            // Add more settings as needed:
            // services.Configure<DbSettings>(configuration.GetSection("DbSettings"));
            // services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

            return services;
        }
    }
}
