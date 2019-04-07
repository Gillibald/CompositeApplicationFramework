#region Usings

using System;
using System.Linq;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    using System.Collections;
    using System.Linq.Expressions;
    using System.Reflection;

    #region Dependencies

    #endregion

    /// <summary>
    ///     Object extension
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        ///     Get first parameter from object parameters
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string GetFirstParameter(this object[] para)
        {
            if (para == null)
            {
                return null;
            }

            if (para.Length == 0)
            {
                return null;
            }

            var firstOrDefault = para.FirstOrDefault();

            return firstOrDefault?.ToString();
        }

        public static T CheckForNull<T>(this T obj, string message) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(typeof (T).Name, message);
            }

            return obj;
        }

        public static T CheckForNull<T>(this T obj, string paramName, string message) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"{typeof (T).Name} : {paramName}", message);
            }

            return obj;
        }

        public static int GetHashCodeByProperties(this object obj, int initialHash = 34, int multiplier = 3)
        {
            var theProperties = obj.GetType().GetProperties();
            var hash = initialHash;
            foreach (var value in theProperties.Where(info => info != null).Select(info => info.GetValue(obj, null)).Where(value => value != null))
            {
                unchecked
                {
                    if (value is DateTime)
                    {
                        hash = multiplier * hash ^ (int)(((DateTime)value).Ticks >> 0x20);
                    }
                    else
                    {
                        hash = multiplier * hash ^ value.GetHashCode();
                    }
                }
            }
            return hash;
        }

        private static readonly Hashtable ClonerCache = Hashtable.Synchronized(new Hashtable());

        public static bool Equal<T>(this T x, T y)
        {
            return EqualCache<T>.Compare(x, y);
        }

        public static object Clone(this object obj, Type type)
        {
            return GetCloner(type)(obj);
        }

        private static Func<object, object> GetCloner(Type type)
        {
            var cloner = (Func<object, object>)ClonerCache[type];

            if (cloner != null) return cloner;

            Register(type);

            return (Func<object, object>)ClonerCache[type];
        }

        public static void Register(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new NotSupportedException("Type has no parameterless constructor");
            }

            lock (ClonerCache.SyncRoot)
            {
                var cloner = (Func<object, object>)ClonerCache[type];

                if (cloner == null)
                {
                    var param = Expression.Parameter(type, "in");

                    var bindings = from prop in type.GetProperties()
                                   where prop.CanRead && prop.CanWrite                                                           
                                   select (MemberBinding)Expression.Bind(prop,
                                       Expression.Property(param, prop));

                    cloner = Expression.Lambda<Func<object, object>>(
                        Expression.MemberInit(
                            Expression.New(type), bindings), param).Compile();

                    ClonerCache.Add(type, cloner);
                }
            }
        }

        public static T Clone<T>(this T obj) where T : new()
        {
            return Cloner<T>.Clone(obj);
        }

        private static class Cloner<T> where T : new()
        {
            private static readonly Func<T, T> CloneFunc;

            static Cloner()
            {
                var param = Expression.Parameter(typeof(T), "in");

                var bindings = from prop in typeof(T).GetProperties()
                               where prop.CanRead && prop.CanWrite
                               select (MemberBinding)Expression.Bind(prop,
                                   Expression.Property(param, prop));

                CloneFunc = Expression.Lambda<Func<T, T>>(
                    Expression.MemberInit(
                        Expression.New(typeof(T)), bindings), param).Compile();
            }

            public static T Clone(T obj)
            {
                return CloneFunc(obj);
            }
        }

        private static class EqualCache<T>
        {
            internal static readonly Func<T, T, bool> Compare;

            static EqualCache()
            {
                var members = typeof(T).GetProperties(
                    BindingFlags.Instance | BindingFlags.Public).Concat(typeof(T).GetFields(
                        BindingFlags.Instance | BindingFlags.Public).Cast<MemberInfo>());
                var x = Expression.Parameter(typeof(T), "x");
                var y = Expression.Parameter(typeof(T), "y");

                Expression body = null;
                foreach (var member in members)
                {
                    Expression memberEqual;
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            memberEqual = Expression.Equal(
                                Expression.Field(x, (FieldInfo)member),
                                Expression.Field(y, (FieldInfo)member));
                            break;
                        case MemberTypes.Property:
                            memberEqual = Expression.Equal(
                                Expression.Property(x, (PropertyInfo)member),
                                Expression.Property(y, (PropertyInfo)member));
                            break;
                        default:
                            throw new NotSupportedException(
                                member.MemberType.ToString());
                    }
                    body = body == null ? memberEqual : Expression.AndAlso(body, memberEqual);
                }
                if (body == null)
                {
                    Compare = delegate { return true; };
                }
                else
                {
                    Compare = Expression.Lambda<Func<T, T, bool>>(body, x, y)
                        .Compile();
                }
            }
        }
    }
}