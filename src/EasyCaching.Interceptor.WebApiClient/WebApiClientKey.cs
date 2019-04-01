using EasyCaching.Core.Interceptor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCaching.Interceptor.WebApiClient
{

    /// <summary>
    /// WebApiClient的key值转换
    /// </summary>
    internal class WebApiClientKey
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// Easycaching able
        /// </summary>
        public EasyCachingAbleAttribute EasyCachingAbleAttribute { get; set; }

        /// <summary>
        /// Easycaching put
        /// </summary>
        public EasyCachingPutAttribute EasyCachingPutAttribute { get; set; }

        /// <summary>
        /// 转成json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 转成WebApiClientKey对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static WebApiClientKey ToObject(string json)
        {
            return JsonConvert.DeserializeObject<WebApiClientKey>(json);

        }
    }
}
