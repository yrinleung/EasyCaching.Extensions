using EasyCaching.Bus.CAP;
using EasyCaching.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EasyCachingOptionsExtensions
    {

        /// <summary>
        /// Withs the CAP bus.
        /// </summary>
        /// <returns>The CAP bus.</returns>
        /// <param name="options">Options.</param>
        /// <param name="configure">Configure.</param>
        public static EasyCachingOptions WithCapBus(this EasyCachingOptions options, Action<CapBusOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            options.RegisterExtension(new CapBusOptionsExtension(configure));
            return options;
        }
    }
}
