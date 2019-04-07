#region Usings

using System.ComponentModel;
using System.Dynamic;

#endregion

namespace CompositeApplicationFramework.Utility.Proxy
{
    public class EditableProxy : DynamicProxy, IEditableObject
    {
        #region private members

        #endregion

        #region protected methods

        protected override void SetMember(string propertyName, object value)
        {
            if (IsEditing)
            {
                BackupState.SetOriginalValue(propertyName, GetPropertyInfo(propertyName).GetValue(ProxiedObject, null));
                BackupState.SetNewValue(propertyName, value);
                RaisePropertyChanged(propertyName);
            }
            else
            {
                base.SetMember(propertyName, value);
            }
        }

        protected override object GetMember(string propertyName)
        {
            return IsEditing && BackupState.NewValues.ContainsKey(propertyName)
                ? BackupState.NewValues[propertyName]
                : base.GetMember(propertyName);
        }

        #endregion

        #region constructor

        public EditableProxy()
        {
        }

        public EditableProxy(object proxiedObject)
            : base(proxiedObject)
        {
        }

        #endregion

        #region public methods

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof (IEditableObject))
            {
                result = this;
                return true;
            }
            return base.TryConvert(binder, out result);
        }

        public object GetValue(string pPropertyName)
        {
            return GetMember(pPropertyName);
        }

        public void SetValue(string pPropertyName, object pValue)
        {
            SetMember(pPropertyName, pValue);
        }

        #endregion

        #region IEditableObject methods

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                BackupState = new BackupState();
            }
        }

        public void CancelEdit()
        {
            if (IsEditing)
            {
                BackupState = null;
            }
        }

        public void EndEdit()
        {
            if (IsEditing)
            {
                var editObject = BackupState;
                BackupState = null;

                foreach (var item in editObject.NewValues)
                {
                    SetMember(item.Key, item.Value);
                }
            }
        }

        #endregion

        #region public properties

        public bool IsEditing => BackupState != null;

        public bool IsChanged => IsEditing && BackupState.NewValues.Count > 0;

        public BackupState BackupState { get; private set; }

        #endregion
    }
}