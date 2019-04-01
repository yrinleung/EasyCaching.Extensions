using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;

namespace EasyCaching.Interceptor.WebApiClient
{
    /// <summary>
    /// EasyCaching提供缓存支持
    /// </summary>
    public class EasyCachingResponseCacheProvider : IResponseCacheProvider
    {
        private IServiceProvider _serviceProvider;
        public EasyCachingResponseCacheProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取提供者的友好名称
        /// </summary>
        public string Name => "EasyCaching";

        /// <summary>
        /// 从缓存中获取响应实体
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<ResponseCacheResult> GetAsync(string key)
        {
            var webApiClientKey = WebApiClientKey.ToObject(key);

            if (webApiClientKey.EasyCachingAbleAttribute == null)
            {
                var result = new ResponseCacheResult(null, false);
                return await Task.FromResult(result);
            }

            var cacheProvider = _serviceProvider.GetService(typeof(IEasyCachingProvider)) as IEasyCachingProvider;

            object cacheValue = await cacheProvider.GetAsync(webApiClientKey.CacheKey, typeof(ResponseCacheEntry));
            if (cacheValue != null)
            {
                var result = new ResponseCacheResult((ResponseCacheEntry)cacheValue, true);
                return await Task.FromResult(result);
            }
            else
            {
                var result = new ResponseCacheResult(null, false);
                return await Task.FromResult(result);
            }
        }

        /// <summary>
        /// 设置响应实体到缓存
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="key">键</param>
        /// <param name="entry">缓存实体</param>
        /// <param name="expiration">有效时间</param>
        /// <returns></returns>
        public async Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration)
        {
            var webApiClientKey = WebApiClientKey.ToObject(key);
            if (webApiClientKey.EasyCachingAbleAttribute != null || webApiClientKey.EasyCachingPutAttribute != null)
            {
                var cacheProvider = _serviceProvider.GetService(typeof(IEasyCachingProvider)) as IEasyCachingProvider;

                await cacheProvider.SetAsync(webApiClientKey.CacheKey, entry, expiration);
            }
        }

    }
}
