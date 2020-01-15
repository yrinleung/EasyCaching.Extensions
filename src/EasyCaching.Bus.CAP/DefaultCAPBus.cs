using DotNetCore.CAP;
using EasyCaching.Core;
using EasyCaching.Core.Bus;
using EasyCaching.HybridCache;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCaching.Bus.CAP
{
    /// <summary>
    /// default CAP bus
    /// </summary>
    public class DefaultCAPBus : EasyCachingAbstractBus
    {
        /// <summary>
        /// CAP publisher
        /// </summary>
        private readonly ICapPublisher _capBus;

        /// <summary>
        /// option
        /// </summary>
        private readonly CapBusOptions _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capBus"></param>
        /// <param name="optionsAccs"></param>
        public DefaultCAPBus(ICapPublisher capBus, CapBusOptions optionsAccs)
        {
            _capBus = capBus;
            _options = optionsAccs;
        }

        /// <summary>
        /// Publish the specified topic and message.
        /// </summary>
        /// <param name="topic">Topic.</param>
        /// <param name="message">Message.</param>
        public override void BasePublish(string topic, EasyCachingMessage message)
        {
            _capBus.Publish(_options.TopicName, message);
        }

        /// <summary>
        /// Publishs the specified topic and message async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="topic">Topic.</param>
        /// <param name="message">Message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public override async Task BasePublishAsync(string topic, EasyCachingMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _capBus.PublishAsync(_options.TopicName, message);
        }

        /// <summary>
        /// Subscribe the specified topic and action.
        /// </summary>
        /// <param name="topic">Topic.</param>
        /// <param name="action">Action.</param>
        public override void BaseSubscribe(string topic, Action<EasyCachingMessage> action)
        {
            //由于CAP是根据ICapSubscribe启动订阅者的，所以系统启动时自动启动订阅者
        }
    }
}
