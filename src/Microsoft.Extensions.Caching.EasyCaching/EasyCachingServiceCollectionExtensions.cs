using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.EasyCaching;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EasyCachingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds EasyCaching distributed caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{EasyCachingOptions}"/> to configure the provided
        /// <see cref="EasyCachingOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddEasyCachingCache(this IServiceCollection services, Action<EasyCachingOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.Add(ServiceDescriptor.Singleton<IDistributedCache, EasyCachingDistributedCache>());

            return services;
        }
    }
}
