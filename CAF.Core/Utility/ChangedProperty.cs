#region Usings

using System;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Types;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class ChangedProperty : IChangedProperty
    {
        public ChangeAction Action { get; set; }
        public string PropertyName { get; set; }
        public object NewValue { get; set; }
        public object OldValue { get; set; }
        public Type PropertyType { get; set; }
    }
}