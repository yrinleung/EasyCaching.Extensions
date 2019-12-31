using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Caching.EasyCaching
{
    /// <summary>
    /// 
    /// </summary>
    public class EasyCachingOptions : IOptions<EasyCachingOptions>
    {
        /// <summary>
        /// 
        /// </summary>
        public string CachingProviderName { get; set; }

        /// <summary>
        /// 默认滑动过期时间,默认20分钟
        /// </summary>
        public TimeSpan DefaultSlidingExpiration { get; set; } = TimeSpan.FromMinutes(20);

        EasyCachingOptions IOptions<EasyCachingOptions>.Value
        {
            get { return this; }
        }
    }
}
