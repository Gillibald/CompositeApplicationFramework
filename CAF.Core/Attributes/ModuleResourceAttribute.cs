#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModuleResourceAttribute : Attribute
    {
        public ModuleResourceAttribute(string resource)
        {
            ResourceLocation = resource;
        }

        public string ResourceLocation { get; set; }
    }
}