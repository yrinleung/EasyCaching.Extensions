using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCaching.Bus.CAP
{
    public class CapBusOptions
    {
        /// <summary>
        /// Gets or sets the name of the topic.
        /// </summary>
        /// <value>The name of the topic.</value>
        public string TopicName { get; set; }

        /// <summary>
        ///  queue prefix name
        /// </summary>
        public string QueuePrefixName { get; set; } = "capmq.queue.undurable.easycaching.subscriber.";
    }
}
