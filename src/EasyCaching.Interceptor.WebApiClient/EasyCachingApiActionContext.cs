using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;

namespace EasyCaching.Interceptor.WebApiClient
{
    public class EasyCachingApiActionContext : ApiActionContext
    {
        private readonly bool IsSetCache;
        private readonly string CacheKey;
        private readonly int Expiration;
        public EasyCachingApiActionContext(IHttpApi httpApi, HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor, bool isSetCache, string cacheKey, int expiration) : base(httpApi, httpApiConfig, apiActionDescriptor)
        {
            IsSetCache = isSetCache;
            CacheKey = cacheKey;
            Expiration = expiration;
        }


        public override async Task<TResult> ExecuteActionAsync<TResult>()
        {
            var result = await base.ExecuteActionAsync<TResult>();

            if (IsSetCache)
            {
                var cacheProvider = base.GetService<IEasyCachingProvider>();

                await cacheProvider.SetAsync(CacheKey, result, TimeSpan.FromSeconds(Expiration));
            }

            return result;
        }
    }
}
