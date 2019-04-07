#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CompositeApplicationFramework.Attributes;

#endregion

namespace CompositeApplicationFramework.Utility.Proxy
{
    public class UpdateProxy<T>
        where T : INotifyPropertyChanged
    {
        private readonly EditableProxy _sourceProxy;
        private readonly EditableProxy _targetProxy;
        private readonly Dictionary<string, PropertyInfo> _allowedProperties;

        public UpdateProxy(T pSource, T pTarget)
        {
            _allowedProperties = pSource.GetType()
                .GetProperties().Where(x =>
                    x.CanRead && x.CanWrite && x.GetCustomAttribute<ExcludePropertyAttribute>() == null &&
                    x.GetCustomAttribute<IgnoreStateAttribute>() == null).ToDictionary(x => x.Name);

            _sourceProxy = new EditableProxy(pSource);
            _targetProxy = new EditableProxy(pTarget);

            _sourceProxy.BeginEdit();

            pSource.PropertyChanged += AssignedObject_PropertyChanged;
        }

        private void AssignedObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var lPropertyName = e.PropertyName;

            if (!_allowedProperties.ContainsKey(lPropertyName)) return;

            var lSourceOldValue = _sourceProxy.BackupState.GetOriginalValue(lPropertyName);
            var lSourceNewValue = _sourceProxy.GetValue(lPropertyName);
            var lTargetValue = _targetProxy.GetValue(lPropertyName);

            if (lTargetValue == null)
            {
                _targetProxy.SetValue(lPropertyName, lSourceNewValue);
            }
            else
            {
                if (lSourceOldValue == null)
                {
                    _targetProxy.SetValue(lPropertyName, lSourceNewValue);
                }
                else
                {
                    if (lTargetValue.Equals(lSourceOldValue))
                    {
                        _targetProxy.SetValue(lPropertyName, lSourceNewValue);
                    }
                }
            }
            _sourceProxy.BackupState.SetOriginalValue(lPropertyName, lSourceNewValue);
        }
    }
}