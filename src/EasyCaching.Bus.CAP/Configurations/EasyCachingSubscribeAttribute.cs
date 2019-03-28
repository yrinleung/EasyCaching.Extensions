using DotNetCore.CAP.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCaching.Bus.CAP
{
    /// <summary>
    /// 
    /// </summary>
    public class EasyCachingSubscribeAttribute : TopicAttribute
    {
        public EasyCachingSubscribeAttribute(string name, string group)
            : base(name)
        {
            this.Group = group;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
