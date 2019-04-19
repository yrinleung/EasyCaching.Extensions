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
        /// 重写http接口调用的拦截器
        /// </summary>
        /// <param name="httpApiConfig"></param>
        public EasyCachingInterceptor(HttpApiConfig httpApiConfig)
            : base(httpApiConfig)
        {
        } 

        /// <summary>
        /// 创建api的描述
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected override ApiActionDescriptor CreateApiActionDescriptor(MethodInfo method)
        {
            return new EasyCachingApiActionDescriptor(method);
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
