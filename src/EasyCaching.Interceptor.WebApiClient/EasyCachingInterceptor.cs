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
    public class EasyCachingInterceptor : ApiInterceptor
    {
        public EasyCachingInterceptor(HttpApiConfig httpApiConfig) : base(httpApiConfig)
        {
        }

        /// <summary>
        /// The typeof task result method.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo>
                    TypeofTaskResultMethod = new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        /// api的描述
        /// </summary>

        private ApiActionDescriptor Descriptor;

        /// <summary>
        /// api的上下文
        /// </summary>
        private ApiActionContext ApiActionContext;

        /// <summary>
        /// 是否需要填充Cache
        /// </summary>
        private bool IsSetCache;

        /// <summary>
        /// Cache Key
        /// </summary>
        private string CacheKey;

        /// <summary>
        /// Cache 到期
        /// </summary>
        private int CacheExpiration = 30;

        /// <summary>
        /// 返回创建EasyCachingApiActionContext新实例
        /// </summary>
        /// <param name="httpApi">httpApi代理类实例</param>
        /// <param name="apiActionDescriptor">api的描述</param>
        /// <returns></returns>
        protected override ApiActionContext CreateApiActionContext(IHttpApi httpApi, ApiActionDescriptor apiActionDescriptor)
        {
            return new EasyCachingApiActionContext(httpApi, this.HttpApiConfig, apiActionDescriptor, IsSetCache, CacheKey, CacheExpiration);
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
            IsSetCache = (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(EasyCachingAbleAttribute)) is EasyCachingAbleAttribute) || (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(EasyCachingPutAttribute)) is EasyCachingPutAttribute);

            var httpApi = target as IHttpApi;
            Descriptor = this.GetApiActionDescriptor(method, parameters);
            ApiActionContext = this.CreateApiActionContext(httpApi, Descriptor);
            
            //Process any early evictions 
            ProcessEvictAsync(method, parameters, true).GetAwaiter().GetResult();

            //Process any cache interceptor 
            var apiTask = ProceedAbleAsync(target, method, parameters).GetAwaiter().GetResult();

            // Process any put requests
            ProcessPut(target, method, parameters);

            // Process any late evictions
            ProcessEvictAsync(method, parameters, false).GetAwaiter().GetResult();

            return apiTask;
        }
        
        /// <summary>
        /// Proceeds the able async.
        /// </summary>
        /// <returns>The able async.</returns>
        private async Task<object> ProceedAbleAsync(object target, MethodInfo method, object[] parameters)
        {
            object apiTask = null;

            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(EasyCachingAbleAttribute)) is EasyCachingAbleAttribute attribute)
            {
                var keyGenerator = ApiActionContext.GetService<IEasyCachingKeyGenerator>();
                var cacheProvider = ApiActionContext.GetService<IEasyCachingProvider>();

                var returnType = Descriptor.Return.DataType.Type;

                CacheKey = keyGenerator.GetCacheKey(method, parameters, attribute.CacheKeyPrefix);
                CacheExpiration = attribute.Expiration;

                object cacheValue = await cacheProvider.GetAsync(CacheKey, returnType);

                if (cacheValue != null)
                {
                    if (Descriptor.Return.IsTaskDefinition)
                    {
                        return
                            TypeofTaskResultMethod.GetOrAdd(returnType, t => typeof(Task).GetMethods().First(p => p.Name == "FromResult" && p.ContainsGenericParameters).MakeGenericMethod(returnType)).Invoke(null, new object[] { cacheValue });
                    }
                    if (Descriptor.Return.IsITaskDefinition)
                    {
                        return
                            TypeofTaskResultMethod.GetOrAdd(returnType, t => typeof(ApiCacheTask<>).MakeGenericType(returnType).GetMethods().First(p => p.Name == "FromResult")).Invoke(null, new object[] { cacheValue });
                    }
                    else
                    {
                        return cacheValue;
                    }
                }
                else
                {
                    apiTask = base.Intercept(target, method, parameters);
                }
            }
            else
            {
                apiTask = base.Intercept(target, method, parameters);
            }
            return apiTask;
        }


        /// <summary>
        /// Processes the put async.
        /// </summary>
        /// <returns>The put</returns>
        private void ProcessPut(object target, MethodInfo method, object[] parameters)
        {
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(EasyCachingPutAttribute)) is EasyCachingPutAttribute attribute)
            {
                var keyGenerator = ApiActionContext.GetService<IEasyCachingKeyGenerator>();
                CacheKey = keyGenerator.GetCacheKey(method, parameters, attribute.CacheKeyPrefix);
                CacheExpiration = attribute.Expiration;
            }
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
                var keyGenerator = ApiActionContext.GetService<IEasyCachingKeyGenerator>();
                var cacheProvider = ApiActionContext.GetService<IEasyCachingProvider>();
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

    }
}
