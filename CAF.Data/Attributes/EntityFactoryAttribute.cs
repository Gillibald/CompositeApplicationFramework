#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityFactoryAttribute : Attribute
    {
        public EntityFactoryAttribute(Type factoryType)
        {
            FactoryType = factoryType;
        }

        public Type FactoryType { get; private set; }
    }
}