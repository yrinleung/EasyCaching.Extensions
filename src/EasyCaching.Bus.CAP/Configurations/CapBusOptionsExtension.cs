using DotNetCore.CAP.Internal;
using EasyCaching.Bus.CAP.Configurations;
using EasyCaching.Core.Bus;
using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyCaching.Bus.CAP
{
    /// <summary>
    /// Redis bus options extension.
    /// </summary>
    internal sealed class CapBusOptionsExtension : IEasyCachingOptionsExtension
    {
        /// <summary>
        /// The configure.
        /// </summary>
        private readonly Action<CapBusOptions> configure;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EasyCaching.Bus.CAP.CapBusOptionsExtension"/> class.
        /// </summary>
        /// <param name="configure">Configure.</param>
        public CapBusOptionsExtension(Action<CapBusOptions> configure)
        {
            this.configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            var option = new CapBusOptions();
            configure(option);
            services.AddSingleton<CapBusOptions>(x => option);
            services.AddSingleton<IEasyCachingBus, DefaultCAPBus>();
            services.AddSingleton<IConsumerServiceSelector, EasyCachingConsumerServiceSelector>();
        }

    }
}
