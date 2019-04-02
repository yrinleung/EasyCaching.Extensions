using System.Reflection;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Defaults;

namespace EasyCaching.Interceptor.WebApiClient
{
    /// <summary>
    /// EasyCaching重写http接口调用的拦截器
    /// </summary>
    class EasyCachingInterceptor : ApiInterceptor
    {
        /// <summary>
        /// ApiActionDescriptor缓存
        /// </summary>
        private static readonly ConcurrentEasyCaching<MethodInfo, EasyCachingApiActionDescriptor> descriptorCache = new ConcurrentEasyCaching<MethodInfo, EasyCachingApiActionDescriptor>();

        /// <summary>
        /// 重写http接口调用的拦截器
        /// </summary>
        /// <param name="httpApiConfig"></param>
        public EasyCachingInterceptor(HttpApiConfig httpApiConfig)
            : base(httpApiConfig)
        {
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

        /// <summary>
        /// 创建请求上下文
        /// </summary>
        /// <param name="httpApi"></param>
        /// <param name="apiActionDescriptor"></param>
        /// <returns></returns>
        protected override ApiActionContext CreateApiActionContext(IHttpApi httpApi, ApiActionDescriptor apiActionDescriptor)
        {
            return new EasyCachingApiActionContext(httpApi, this.HttpApiConfig, apiActionDescriptor);
        }
    }
}
