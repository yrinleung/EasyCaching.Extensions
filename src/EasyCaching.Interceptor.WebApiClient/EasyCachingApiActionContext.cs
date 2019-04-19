using EasyCaching.Core;
using EasyCaching.Core.Interceptor;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;

namespace EasyCaching.Interceptor.WebApiClient
{
    /// <summary>
    /// 表示EasyCaching的请求上下文
    /// </summary>
    class EasyCachingApiActionContext : ApiActionContext
    {
        /// <summary>
        /// EasyCaching的请求上下文
        /// </summary>
        /// <param name="httpApi"></param>
        /// <param name="httpApiConfig"></param>
        /// <param name="apiActionDescriptor"></param>
        public EasyCachingApiActionContext(IHttpApi httpApi, HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
            : base(httpApi, httpApiConfig, apiActionDescriptor)
        {
        }

        /// <summary>
        /// 执行上下文的请求方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async override Task<TResult> ExecuteActionAsync<TResult>()
        {
            try
            {
                await this.ProcessEvictAsync(isBefore: true).ConfigureAwait(false);
                return await base.ExecuteActionAsync<TResult>();
            }
            finally
            {
                await this.ProcessEvictAsync(isBefore: false).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Processes the evict async.
        /// </summary>
        /// <returns>The evict async.</returns>
        /// <param name="isBefore">If set to <c>true</c> is before.</param>
        private async Task ProcessEvictAsync(bool isBefore)
        {
            var apiActionDescriptor = this.ApiActionDescriptor as EasyCachingApiActionDescriptor;
            var attribute = apiActionDescriptor.EasyCachingEvictAttribute;
            if (attribute == null || attribute.IsBefore != isBefore)
            {
                return;
            }

            var keyGenerator = this.GetService<IEasyCachingKeyGenerator>();
            var cacheProvider = this.GetService<IEasyCachingProvider>();
            if (attribute.IsAll == true)
            {
                //If is all , clear all cached items which cachekey start with the prefix.
                var cachePrefix = keyGenerator.GetCacheKeyPrefix(apiActionDescriptor.Member, attribute.CacheKeyPrefix);
                await cacheProvider.RemoveByPrefixAsync(cachePrefix);
            }
            else
            {
                //If not all , just remove the cached item by its cachekey.
                var arguments = apiActionDescriptor.Arguments;
                var cacheKey = keyGenerator.GetCacheKey(apiActionDescriptor.Member, arguments, attribute.CacheKeyPrefix);
                await cacheProvider.RemoveAsync(cacheKey);
            }
        }
    }
}
