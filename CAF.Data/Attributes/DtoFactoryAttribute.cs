#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DtoFactoryAttribute : Attribute
    {
        public DtoFactoryAttribute(Type factoryType)
        {
            FactoryType = factoryType;
        }

        public Type FactoryType { get; private set; }
    }
}