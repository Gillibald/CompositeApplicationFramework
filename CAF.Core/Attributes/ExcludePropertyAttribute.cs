#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludePropertyAttribute : Attribute
    {
    }
}