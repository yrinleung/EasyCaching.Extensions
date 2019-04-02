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
    class EasyCachingResponseCacheProvider : IResponseCacheProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 获取提供者的友好名称
        /// </summary>
        public string Name => "EasyCaching";

        /// <summary>
        /// EasyCaching提供缓存支持
        /// </summary>
        /// <param name="serviceProvider"></param>
        public EasyCachingResponseCacheProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        /// <summary>
        /// 从缓存中获取响应实体
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<ResponseCacheResult> GetAsync(string key)
        {
            var cacheProvider = _serviceProvider.GetService(typeof(IEasyCachingProvider)) as IEasyCachingProvider;
            var value = await cacheProvider.GetAsync(key, typeof(ResponseCacheEntry));
            var cacheValue = value as ResponseCacheEntry;

            if (cacheValue == null)
            {
                return ResponseCacheResult.NoValue;
            }
            return new ResponseCacheResult(cacheValue, true);
        }

        /// <summary>
        /// 设置响应实体到缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entry">缓存实体</param>
        /// <param name="expiration">有效时间</param>
        /// <returns></returns>
        public async Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration)
        {
            var cacheProvider = _serviceProvider.GetService(typeof(IEasyCachingProvider)) as IEasyCachingProvider;
            await cacheProvider.SetAsync(key, entry, expiration);
        }
    }
}
