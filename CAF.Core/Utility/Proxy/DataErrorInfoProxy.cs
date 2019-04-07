#region Usings

using System;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Text;

#endregion

namespace CompositeApplicationFramework.Utility.Proxy
{
    public class DataErrorInfoProxy : ValidatingProxy, IDataErrorInfo
    {
        protected override bool Validate(PropertyInfo propertyInfo, object value)
        {
            var returnValue = base.Validate(propertyInfo, value);
            return returnValue;
        }

        #region public methods

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof (IDataErrorInfo))
            {
                result = this;
                return true;
            }
            return base.TryConvert(binder, out result);
        }

        #endregion

        #region constructor

        public DataErrorInfoProxy()
        {
        }

        public DataErrorInfoProxy(object proxiedObject)
            : base(proxiedObject)
        {
        }

        #endregion

        #region IDataErrorInfo Member

        public string Error
        {
            get
            {
                var returnValue = new StringBuilder();

                foreach (var item in validationResults)
                {
                    foreach (var validationResult in item.Value)
                    {
                        returnValue.AppendLine(validationResult.ErrorMessage);
                    }
                }

                return returnValue.ToString();
            }
        }

        public string this[string columnName] => validationResults.ContainsKey(columnName)
            ? string.Join(Environment.NewLine, validationResults[columnName])
            : string.Empty;

        #endregion
    }
}