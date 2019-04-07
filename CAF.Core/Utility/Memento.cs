#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class Memento<T>
    {
        public Memento(T originator)
        {
            var propertyInfos =
                typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in propertyInfos)
            {
                StoredProperties[property] = property.GetValue(originator, null);
            }
        }

        public Dictionary<PropertyInfo, object> StoredProperties { get; } = new Dictionary<PropertyInfo, object>();

        public void Restore(T originator)
        {
            foreach (var pair in StoredProperties)
            {
                pair.Key.SetValue(originator, pair.Value, null);
            }
        }
    }
}