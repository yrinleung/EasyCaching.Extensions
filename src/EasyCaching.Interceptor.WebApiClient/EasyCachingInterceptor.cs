using EasyCaching.Core;
using EasyCaching.Core.Interceptor;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Defaults;

namespace EasyCaching.Interceptor.WebApiClient
{
    /// <summary>
    /// EasyCaching重写http接口调用的拦截器
    /// </summary>
    public class EasyCachingInterceptor : ApiInterceptor
    {
        /// <summary>
        /// ApiActionDescriptor缓存
        /// </summary>
        private static readonly ConcurrentEasyCaching<MethodInfo, EasyCachingApiActionDescriptor> descriptorCache;

        public EasyCachingInterceptor(HttpApiConfig httpApiConfig) : base(httpApiConfig)
        {

        }

        /// <summary>
        /// EasyCaching重写http接口调用的拦截器
        /// </summary>
        static EasyCachingInterceptor()
        {
            descriptorCache = new ConcurrentEasyCaching<MethodInfo, EasyCachingApiActionDescriptor>();
        }
        

        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="target">接口的实例</param>
        /// <param name="method">接口的方法</param>
        /// <param name="parameters">接口的参数集合</param>
        /// <returns></returns>
        public override object Intercept(object target, MethodInfo method, object[] parameters)
        {
            //Process any early evictions 
            ProcessEvictAsync(method, parameters, true).GetAwaiter().GetResult();

            var apiTask = base.Intercept(target, method, parameters);

            // Process any late evictions
            ProcessEvictAsync(method, parameters, false).GetAwaiter().GetResult();

            return apiTask;
        }
        
        /// <summary>
        /// Processes the evict async.
        /// </summary>
        /// <returns>The evict async.</returns>
        /// <param name="method">method.</param>
        /// <param name="parameters">parameters.</param>
        /// <param name="isBefore">If set to <c>true</c> is before.</param>
        private async Task ProcessEvictAsync(MethodInfo method, object[] parameters, bool isBefore)
        {
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(EasyCachingEvictAttribute)) is EasyCachingEvictAttribute attribute && attribute.IsBefore == isBefore)
            {
                var keyGenerator = HttpApiConfig.ServiceProvider.GetService(typeof(IEasyCachingKeyGenerator)) as IEasyCachingKeyGenerator;
                var cacheProvider = HttpApiConfig.ServiceProvider.GetService(typeof(IEasyCachingProvider)) as IEasyCachingProvider;
                if (attribute.IsAll)
                {
                    //If is all , clear all cached items which cachekey start with the prefix.
                    var cachePrefix = keyGenerator.GetCacheKeyPrefix(method, attribute.CacheKeyPrefix);

                    await cacheProvider.RemoveByPrefixAsync(cachePrefix);
                }
                else
                {
                    //If not all , just remove the cached item by its cachekey.
                    var cacheKey = keyGenerator.GetCacheKey(method, parameters, attribute.CacheKeyPrefix);

                    await cacheProvider.RemoveAsync(cacheKey);
                }
            }
        }


        /// <summary>
        /// 获取api的描述
        /// 默认实现使用了缓存
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="parameters">参数值集合</param>
        /// <returns></returns>
        protected override ApiActionDescriptor GetApiActionDescriptor(MethodInfo method, object[] parameters)
        {
            return descriptorCache.GetOrAdd(method, m => new EasyCachingApiActionDescriptor(m)).Clone(parameters);
        }
    }
}
