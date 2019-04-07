#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

#endregion

namespace CompositeApplicationFramework.Utility.Proxy
{
    public class CustomValidatingProxy : ValidatingProxy
    {
        #region public properties

        public ValidationAttributesCollection ValidationAttributes { get; }

        #endregion

        #region protected methods

        protected override IEnumerable<ValidationAttribute> GetValidationAttributes(PropertyInfo propertyInfo)
        {
            var returnValue = base.GetValidationAttributes(propertyInfo);

            if (ValidationAttributes.HasAttributes(propertyInfo.Name))
            {
                returnValue = returnValue.Concat(ValidationAttributes[propertyInfo.Name]);
            }

            return returnValue;
        }

        #endregion

        #region public nested class

        public class ValidationAttributesCollection
        {
            #region private members

            private readonly Dictionary<string, List<ValidationAttribute>> _customValidationAttributes =
                new Dictionary<string, List<ValidationAttribute>>();

            #endregion

            #region public properties

            public List<ValidationAttribute> this[string propertyName]
            {
                get
                {
                    List<ValidationAttribute> returnValue;

                    if (!_customValidationAttributes.TryGetValue(propertyName, out returnValue))
                    {
                        returnValue = new List<ValidationAttribute>();
                        _customValidationAttributes.Add(propertyName, returnValue);
                    }

                    return returnValue;
                }

                set
                {
                    if (_customValidationAttributes.ContainsKey(propertyName))
                    {
                        _customValidationAttributes.Add(propertyName, value);
                    }
                    else
                    {
                        _customValidationAttributes[propertyName] = value;
                    }
                }
            }

            #endregion

            #region public methods

            public bool HasAttributes(string propertyName)
            {
                return _customValidationAttributes.ContainsKey(propertyName)
                       && _customValidationAttributes[propertyName].Count > 0;
            }

            public void Clear(string propertyName)
            {
                if (_customValidationAttributes.ContainsKey(propertyName))
                {
                    _customValidationAttributes.Remove(propertyName);
                }
            }

            public void Clear()
            {
                _customValidationAttributes.Clear();
            }

            #endregion
        }

        #endregion

        #region constructor

        public CustomValidatingProxy()
        {
            ValidationAttributes = new ValidationAttributesCollection();
        }

        public CustomValidatingProxy(object proxiedObject)
            : base(proxiedObject)
        {
            ValidationAttributes = new ValidationAttributesCollection();
        }

        #endregion
    }
}