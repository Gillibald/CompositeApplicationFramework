#region Usings

using System;
using System.ComponentModel;

#endregion

namespace CompositeApplicationFramework.Utility
{
    internal class InternalPropertyDescriptorFactory : TypeConverter
    {
        public static PropertyDescriptor CreatePropertyDescriptor<TComponent, TProperty>(
            string name,
            Func<TComponent, TProperty> getter,
            Action<TComponent, TProperty> setter)
        {
            return new GenericPropertyDescriptor<TComponent, TProperty>(name, getter, setter);
        }

        public static PropertyDescriptor CreatePropertyDescriptor<TComponent, TProperty>(
            string name,
            Func<TComponent, TProperty> getter)
        {
            return new GenericPropertyDescriptor<TComponent, TProperty>(name, getter);
        }

        public static PropertyDescriptor CreatePropertyDescriptor(
            string name,
            Type componentType,
            Type propertyType,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            return new GenericPropertyDescriptor(name, componentType, propertyType, getter, setter);
        }

        public static PropertyDescriptor CreatePropertyDescriptor(
            string name,
            Type componentType,
            Type propertyType,
            Func<object, object> getter)
        {
            return new GenericPropertyDescriptor(name, componentType, propertyType, getter);
        }

        protected class GenericPropertyDescriptor<TComponent, TProperty> : SimplePropertyDescriptor
        {
            private readonly Func<TComponent, TProperty> _getter;
            private readonly Action<TComponent, TProperty> _setter;

            public GenericPropertyDescriptor(
                string name,
                Func<TComponent, TProperty> getter,
                Action<TComponent, TProperty> setter)
                : base(typeof (TComponent), name, typeof (TProperty))
            {
                if (getter == null)
                {
                    throw new ArgumentNullException(nameof(getter));
                }
                if (setter == null)
                {
                    throw new ArgumentNullException(nameof(setter));
                }

                _getter = getter;
                _setter = setter;
            }

            public GenericPropertyDescriptor(string name, Func<TComponent, TProperty> getter)
                : base(typeof (TComponent), name, typeof (TProperty))
            {
                if (getter == null)
                {
                    throw new ArgumentNullException(nameof(getter));
                }

                _getter = getter;
            }

            public override bool IsReadOnly => _setter == null;

            public override object GetValue(object target)
            {
                var component = (TComponent) target;
                var value = _getter(component);
                return value;
            }

            public override void SetValue(object target, object value)
            {
                if (IsReadOnly) return;
                var component = (TComponent) target;
                var newValue = (TProperty) value;
                _setter(component, newValue);
            }
        }

        protected class GenericPropertyDescriptor : SimplePropertyDescriptor
        {
            private readonly Func<object, object> _getter;
            private readonly Action<object, object> _setter;

            public GenericPropertyDescriptor(
                string name,
                Type componentType,
                Type propertyType,
                Func<object, object> getter,
                Action<object, object> setter)
                : base(componentType, name, propertyType)
            {
                if (getter == null)
                {
                    throw new ArgumentNullException(nameof(getter));
                }
                if (setter == null)
                {
                    throw new ArgumentNullException(nameof(setter));
                }

                _getter = getter;
                _setter = setter;
            }

            public GenericPropertyDescriptor(
                string name,
                Type componentType,
                Type propertyType,
                Func<object, object> getter)
                : base(componentType, name, propertyType)
            {
                if (getter == null)
                {
                    throw new ArgumentNullException(nameof(getter));
                }

                _getter = getter;
            }

            public override bool IsReadOnly => _setter == null;

            public override object GetValue(object target)
            {
                var value = _getter(target);
                return value;
            }

            public override void SetValue(object target, object value)
            {
                if (IsReadOnly) return;
                var newValue = value;
                _setter(target, newValue);
            }
        }
    }
}