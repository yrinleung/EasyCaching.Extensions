using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCaching.Bus.CAP
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceLocator
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
