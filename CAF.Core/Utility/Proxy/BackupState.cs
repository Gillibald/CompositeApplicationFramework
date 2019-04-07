#region Usings

using System.Collections.Generic;

#endregion

namespace CompositeApplicationFramework.Utility.Proxy
{
    public class BackupState
    {
        public BackupState()
        {
            OriginalValues = new Dictionary<string, object>();
            NewValues = new Dictionary<string, object>();
        }

        public Dictionary<string, object> OriginalValues { get; }
        public Dictionary<string, object> NewValues { get; }

        public void SetOriginalValue(string propertyName, object value)
        {
            if (OriginalValues.ContainsKey(propertyName))
            {
                OriginalValues[propertyName] = value;
            }
            else
            {
                OriginalValues.Add(propertyName, value);
            }
        }

        public void SetNewValue(string propertyName, object value)
        {
            if (OriginalValues.ContainsKey(propertyName) && OriginalValues[propertyName] == value)
            {
                return;
            }

            if (NewValues.ContainsKey(propertyName))
            {
                NewValues[propertyName] = value;
            }
            else
            {
                NewValues.Add(propertyName, value);
            }
        }

        public object GetOriginalValue(string propertyName)
        {
            return OriginalValues.ContainsKey(propertyName) ? OriginalValues[propertyName] : null;
        }

        public object GetNewValue(string propertyName)
        {
            return NewValues.ContainsKey(propertyName) ? NewValues[propertyName] : null;
        }
    }
}