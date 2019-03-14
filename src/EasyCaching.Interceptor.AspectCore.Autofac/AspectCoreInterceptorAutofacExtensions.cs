using AspectCore.Configuration;
using AspectCore.Extensions.Autofac;
using EasyCaching.Core.Interceptor;
using EasyCaching.Interceptor.AspectCore;
using System.Linq;
using System.Reflection;

namespace Autofac.Extensions.DependencyInjection
{
    public static class AspectCoreInterceptorAutofacExtensions
    {
        /// <summary>
        /// Add the AspectCore interceptor.
        /// </summary>
        public static void AddAspectCoreInterceptor(this ContainerBuilder builder)
        {
            builder.RegisterType<DefaultEasyCachingKeyGenerator>().As<IEasyCachingKeyGenerator>();

            builder.RegisterType<EasyCachingInterceptor>();

            builder.RegisterDynamicProxy(config =>
            {
                bool all(MethodInfo x) => x.CustomAttributes.Any(data => typeof(EasyCachingInterceptorAttribute).GetTypeInfo().IsAssignableFrom(data.AttributeType));

                config.Interceptors.AddTyped<EasyCachingInterceptor>(all);
            });
        }
    }
}
