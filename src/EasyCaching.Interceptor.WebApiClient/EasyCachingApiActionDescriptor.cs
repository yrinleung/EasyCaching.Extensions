using EasyCaching.Core.Interceptor;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;

namespace EasyCaching.Interceptor.WebApiClient
{
    /// <summary>
    /// 重写请求Api描述
    /// </summary>
    class EasyCachingApiActionDescriptor : ApiActionDescriptor
    {
        /// <summary>
        /// 获取声明的EasyCachingEvictAttribute
        /// </summary>
        public EasyCachingEvictAttribute EasyCachingEvictAttribute { get; private set; }


        /// <summary>
        /// 请求Api描述
        /// </summary>
        protected EasyCachingApiActionDescriptor()
        {
        }

        /// <summary>
        /// 请求Api描述
        /// </summary>
        /// <param name="method"></param>
        public EasyCachingApiActionDescriptor(MethodInfo method) : base(method)
        {
            this.EasyCachingEvictAttribute = method.GetCustomAttribute<EasyCachingEvictAttribute>();

            var easyCachingPutAttribute = method.GetCustomAttribute<EasyCachingPutAttribute>();
            var easyCachingAbleAttribute = method.GetCustomAttribute<EasyCachingAbleAttribute>();

            // 不设置Cache属性，Cache调用将不被触发
            if (easyCachingPutAttribute != null || easyCachingAbleAttribute != null)
            {
                this.Cache = new EasyCachingActionCacheAttribute(easyCachingAbleAttribute, easyCachingPutAttribute);
            }
        }

        /// <summary>
        /// 克隆自身
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public override ApiActionDescriptor Clone(object[] parameterValues)
        {
            return new EasyCachingApiActionDescriptor
            {
                Attributes = this.Attributes,
                Cache = this.Cache,
                Filters = this.Filters,
                Member = this.Member,
                Name = this.Name,
                Return = this.Return,
                Parameters = this.Parameters.Select((p, i) => p.Clone(parameterValues[i])).ToArray(),
                EasyCachingEvictAttribute = this.EasyCachingEvictAttribute,
            };
        }

        /// <summary>
        /// 缓存特性
        /// </summary>
        [DebuggerDisplay("Expiration = {Expiration}")]
        private class EasyCachingActionCacheAttribute : Attribute, IApiActionCacheAttribute
        {
            /// <summary>
            /// key前缀
            /// </summary>
            private readonly string prefix;

            /// <summary>
            /// 获取缓存的时间戳
            /// </summary>
            public TimeSpan Expiration { get; } = TimeSpan.FromSeconds(30);

            /// <summary>
            /// 缓存特性
            /// </summary>
            /// <param name="easyCachingAble"></param>
            /// <param name="easyCachingPut"></param>        
            public EasyCachingActionCacheAttribute(EasyCachingAbleAttribute easyCachingAble, EasyCachingPutAttribute easyCachingPut)
            {
                if (easyCachingAble != null)
                {
                    this.prefix = easyCachingAble.CacheKeyPrefix;
                    this.Expiration = TimeSpan.FromSeconds(easyCachingAble.Expiration);
                }
                else if (easyCachingPut != null)
                {
                    this.prefix = easyCachingPut.CacheKeyPrefix;
                    this.Expiration = TimeSpan.FromSeconds(easyCachingPut.Expiration);
                }
            }

            /// <summary>
            /// 获取缓存key
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public Task<string> GetCacheKeyAsync(ApiActionContext context)
            {
                var method = context.ApiActionDescriptor.Member;
                var keyGenerator = context.GetService<IEasyCachingKeyGenerator>();
                var parameters = context.ApiActionDescriptor.Parameters.Select(x => x.Value).ToArray();

                var cacheKey = keyGenerator.GetCacheKey(method, parameters, this.prefix);
                return Task.FromResult(cacheKey);
            }
        }
    }
}
