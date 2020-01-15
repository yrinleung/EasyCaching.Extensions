using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyCaching.Bus.CAP.Configurations
{
    /// <summary>
    /// 重写CAP的消费者服务查询
    /// </summary>
    public class EasyCachingConsumerServiceSelector : ConsumerServiceSelector
    {
        private readonly CapBusOptions _options;
        public EasyCachingConsumerServiceSelector(CapBusOptions options,IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _options = options;
        }

        /// <summary>
        /// 从接口type中查找消费者
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        protected override IEnumerable<ConsumerExecutorDescriptor> FindConsumersFromInterfaceTypes(IServiceProvider provider)
        {
            var executorDescriptorList = new List<ConsumerExecutorDescriptor>();

            executorDescriptorList.AddRange(base.FindConsumersFromInterfaceTypes(provider));

            executorDescriptorList.AddRange(GetDescription(typeof(CacheSubscribe).GetTypeInfo()));

            return executorDescriptorList;
        }

        /// <summary>
        /// 获取消费者标记
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        protected IEnumerable<ConsumerExecutorDescriptor> GetDescription(TypeInfo typeInfo)
        {
            foreach (var method in typeInfo.DeclaredMethods)
            {
                var parameters = method.GetParameters();

                var parameterList = method.GetParameters()
                    .Select(parameter => new ParameterDescriptor
                    {
                        Name = parameter.Name,
                        ParameterType = parameter.ParameterType,
                        IsFromCap = parameter.GetCustomAttributes(typeof(FromCapAttribute)).Any()
                    }).ToList();

                foreach (var parameter in parameters)
                {
                    var type = parameter.ParameterType;

                    var attr = new CapSubscribeAttribute(_options.TopicName);
                    attr.Group = GetQueueName();

                    yield return InitDescriptor(attr, method, typeInfo, parameterList);
                }
            }
        }

        /// <summary>
        /// 初始化消费者标记
        /// </summary>
        /// <returns></returns>
        private ConsumerExecutorDescriptor InitDescriptor(
            TopicAttribute attr,
            MethodInfo methodInfo,
            TypeInfo implType,
            IList<ParameterDescriptor> parameters)
        {
            var descriptor = new ConsumerExecutorDescriptor
            {
                Attribute = attr,
                MethodInfo = methodInfo,
                ImplTypeInfo = implType,
                Parameters = parameters
            };

            return descriptor;
        }

        /// <summary>
        /// 获取队列后缀名
        /// </summary>
        /// <returns></returns>
        private string GetQueueName()
        {
            //计算机名称和程序运行路径组成唯一标示
            //docker运行时每个容器中计算机名称都是不同的
            var queueSuffixName = (Environment.MachineName + "|" + Assembly.GetEntryAssembly().Location).ToMd5().ToLower();

            return $"{_options.QueuePrefixName}.{queueSuffixName}";
        }
    }
}
