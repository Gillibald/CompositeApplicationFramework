#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegionAttribute : Attribute
    {
        public RegionAttribute(string region = "MainRegion")
        {
            Region = region;
        }

        public string Region { get; }
    }
}