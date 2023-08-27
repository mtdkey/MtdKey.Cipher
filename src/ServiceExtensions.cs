using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace MtdKey.Cipher
{

    public static class ServiceExtensions
    {
        /// <summary>
        /// The extension for <see cref="IServiceCollection"/> to use the library with dependency injection.
        /// </summary>
        public static IServiceCollection AddAesMangerService(this IServiceCollection services, Action<AesOptions> aesOptions)
        {
            services.Configure(aesOptions);
            services.TryAddScoped<IAesManager, AesManager>();

            return services;
        }
    }
}
