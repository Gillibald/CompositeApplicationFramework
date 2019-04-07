#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace CompositeApplicationFramework.Helper
{
    public class TypeHelper
    {
        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            var enumerable = source as IList<object> ?? source.Cast<object>().ToList();
            var type = GetCommonBaseClass(enumerable);
            var listType = typeof (List<>).MakeGenericType(type);
            var addMethod = listType.GetMethod("Add");
            var constructorInfo = listType.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                return new List<object>();
            }
            var list = constructorInfo.Invoke(null);

            foreach (var o in enumerable)
            {
                addMethod.Invoke(list, new[] {o});
            }

            return (IEnumerable) list;
        }

        public static Type GetCommonBaseClass(IEnumerable e)
        {
            var types = e.Cast<object>().Select(o => o.GetType()).ToArray();
            return GetCommonBaseClass(types);
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof (object);
            }

            var ret = types[0];

            for (var i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(ret))
                {
                    ret = types[i];
                }
                else
                {
                    // This will always terminate when ret == typeof(object)
                    while (ret != null && !ret.IsAssignableFrom(types[i]))
                    {
                        ret = ret.BaseType;
                    }
                }
            }

            return ret;
        }
    }
}