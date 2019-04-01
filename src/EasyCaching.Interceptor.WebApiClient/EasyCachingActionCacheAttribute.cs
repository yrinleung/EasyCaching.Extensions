using EasyCaching.Core;
using EasyCaching.Core.Interceptor;
using EasyCaching.Core.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;

namespace EasyCaching.Interceptor.WebApiClient
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Expiration = {Expiration}")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EasyCachingActionCacheAttribute : Attribute, IApiActionCacheAttribute
    {
        /// <summary>
        /// 获取缓存的时间戳
        /// </summary>
        public TimeSpan Expiration { get; private set; }

        /// <summary>
        /// 获取缓存key
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<string> GetCacheKeyAsync(ApiActionContext context)
        {
            var method = context.ApiActionDescriptor.Member;

            var parameters = context.ApiActionDescriptor.Parameters.Select(x => x.Value);

            var easyCachingAbleAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(EasyCachingAbleAttribute)) as EasyCachingAbleAttribute;
            var easyCachingPutAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(EasyCachingPutAttribute)) as EasyCachingPutAttribute; 

            var cacheKeyPrefix = string.Empty;
            var easyCachingExpiration = 30;
            var cacheKey = string.Empty;
            if (easyCachingAbleAttribute != null)
            {
                cacheKeyPrefix = easyCachingAbleAttribute.CacheKeyPrefix;
                easyCachingExpiration = easyCachingAbleAttribute.Expiration;
            }
            else if (easyCachingPutAttribute != null)
            {
                cacheKeyPrefix = easyCachingPutAttribute.CacheKeyPrefix;
                easyCachingExpiration = easyCachingPutAttribute.Expiration;
            }

            if (easyCachingAbleAttribute != null || easyCachingPutAttribute != null)
            {
                var keyGenerator = context.GetService<IEasyCachingKeyGenerator>();

                cacheKey = keyGenerator.GetCacheKey(method, parameters.ToArray(), cacheKeyPrefix);

                this.Expiration = TimeSpan.FromSeconds(easyCachingExpiration);
            }


            var easyCachingSerializer = context.GetService<IEasyCachingSerializer>();

            var webApiClientKey = new WebApiClientKey()
            {
                CacheKey = cacheKey,
                EasyCachingAbleAttribute = easyCachingAbleAttribute,
                EasyCachingPutAttribute = easyCachingPutAttribute
            };

            return Task.FromResult(JsonConvert.SerializeObject(webApiClientKey));
        }
    }

}
