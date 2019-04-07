#region Usings

using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using CompositeApplicationFramework.Attributes;

#endregion

namespace CompositeApplicationFramework.Utility.Proxy
{
    public class DynamicProxy : DynamicObject, INotifyPropertyChanged
    {
        #region Private Member

        private readonly PropertyDescriptorCollection mPropertyDescriptors;

        #endregion

        #region public properties

        public object ProxiedObject { get; set; }

        #endregion

        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(INotifyPropertyChanged))
            {
                result = this;
                return true;
            }
            if (ProxiedObject != null && binder.Type.IsInstanceOfType(ProxiedObject))
            {
                result = ProxiedObject;
                return true;
            }
            return base.TryConvert(binder, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetMember(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetMember(binder.Name, value);
            return true;
        }

        #region protected methods

        protected PropertyInfo GetPropertyInfo(string propertyName)
        {
            return
                ProxiedObject.GetType()
                    .GetProperties()
                    .FirstOrDefault(propertyInfo =>
                        propertyInfo.Name == propertyName);
        }

        protected virtual void SetMember(string propertyName, object value)
        {
            try
            {
                var propertyInfo = GetPropertyInfo(propertyName);

                if (propertyInfo == null) return;               

                if (propertyInfo.PropertyType == value.GetType())
                {
                    propertyInfo.SetValue(ProxiedObject, value, null);
                }
                else
                {
                    var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);

                    if (underlyingType != null)
                    {
                        var propertyDescriptor = mPropertyDescriptors.Find(propertyName, false);

                        var converter = propertyDescriptor.Converter;
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                        {
                            var convertedValue = converter.ConvertFrom(value);
                            propertyInfo.SetValue(ProxiedObject, convertedValue, null);
                        }
                    }

                    //Todo: Property Map erstellen
                }

                RaisePropertyChanged(propertyName);
            }
            catch (Exception ex)
            {
                throw new NoSetterFoundExeption(ex) { TargetType = ProxiedObject.GetType(), PropertyName = propertyName };
            }
        }

        private class NoSetterFoundExeption : Exception
        {
            public NoSetterFoundExeption(Exception inner) : base("Setter not found.", inner)
            {

            }

            public Type TargetType { get; set; }

            public string PropertyName { get; set; }
        }

        private class NoGetterFoundExeption : Exception
        {
            public NoGetterFoundExeption(Exception inner) : base("Getter not found.", inner)
            {

            }

            public Type TargetType { get; set; }

            public string PropertyName { get; set; }
        }

        protected virtual object GetMember(string propertyName)
        {
            try
            {
                var propertyInfo = GetPropertyInfo(propertyName);

                return propertyInfo.GetValue(ProxiedObject, null);
            }
            catch (Exception ex)
            {
                throw new NoGetterFoundExeption(ex) { TargetType = ProxiedObject.GetType(), PropertyName = propertyName };
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        #endregion

        #region constructor

        public DynamicProxy()
        {
        }

        public DynamicProxy(object proxiedObject)
        {
            ProxiedObject = proxiedObject;
            mPropertyDescriptors = TypeDescriptor.GetProperties(ProxiedObject.GetType());
        }

        #endregion
    }
}