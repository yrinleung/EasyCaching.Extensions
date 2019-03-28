using DotNetCore.CAP;
using EasyCaching.Core.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCaching.Bus.CAP
{
    /// <summary>
    /// 缓存订阅者
    /// </summary>
    public class CacheSubscribe : ICapSubscribe
    {
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveMessage(EasyCachingMessage message)
        {
            var subscribe = ServiceLocator.ServiceProvider.GetService(typeof(IEasyCachingBus)) as DefaultCAPBus;
            if (subscribe == null)
            {
                return;
            }
            subscribe.BaseOnMessage(message);
        }

    }
}
