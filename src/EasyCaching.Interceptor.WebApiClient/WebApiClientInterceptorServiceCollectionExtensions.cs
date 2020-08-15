using EasyCaching.Interceptor.WebApiClient;
using System;
using WebApiClient;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// WebApiClient interceptor service collection extensions.
    /// </summary>
    public static class WebApiClientInterceptorServiceCollectionExtensions
    {
        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebApiClientUseHttpClientFactory<TInterface>(this IServiceCollection services)
            where TInterface : class, IHttpApi
        {
            return services.AddWebApiClientUseHttpClientFactory<TInterface>(c => { });
        }

        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="config">http接口的配置</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IServiceCollection AddWebApiClientUseHttpClientFactory<TInterface>(this IServiceCollection services, Action<HttpApiConfig> config)
            where TInterface : class, IHttpApi
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return services.AddWebApiClientUseHttpClientFactory<TInterface>((c, p) => config.Invoke(c));
        }

        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebApiClientUseHttpClientFactory<TInterface>(this IServiceCollection services, Action<HttpApiConfig, IServiceProvider> config)
            where TInterface : class, IHttpApi
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var name = typeof(TInterface).ToString();
            services
                .AddHttpClient(name)
                .AddTypedClient<TInterface>((httpClient, provider) =>
                {
                    var httpApiConfig = new HttpApiConfig(httpClient)
                    {
                        ServiceProvider = provider,
                        ResponseCacheProvider = new EasyCachingResponseCacheProvider(provider)
                    };
                    config.Invoke(httpApiConfig, provider);
                    var interceptor = new EasyCachingInterceptor(httpApiConfig);
                    return HttpApi.Create(typeof(TInterface), interceptor) as TInterface;
                });

            return services;
        }
    }
}
