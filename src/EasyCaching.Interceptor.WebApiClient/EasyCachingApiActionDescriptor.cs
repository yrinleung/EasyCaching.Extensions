using EasyCaching.Core.Interceptor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WebApiClient.Contexts;

namespace EasyCaching.Interceptor.WebApiClient
{
    /// <summary>
    /// 重写请求Api描述
    /// </summary>
    public class EasyCachingApiActionDescriptor : ApiActionDescriptor
    {
        public EasyCachingApiActionDescriptor(MethodInfo method) : base(method)
        {
            this.Cache = new EasyCachingActionCacheAttribute();
        }

    }

}
