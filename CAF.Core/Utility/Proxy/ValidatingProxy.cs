#region Usings

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

#endregion

namespace CompositeApplicationFramework.Utility.Proxy
{
    public class ValidatingProxy : EditableProxy
    {
        #region protected members

        protected Dictionary<string, Collection<ValidationResult>> validationResults =
            new Dictionary<string, Collection<ValidationResult>>();

        #endregion

        #region protected methods

        protected override void SetMember(string propertyName, object value)
        {
            if (ValidateOnChange)
            {
                Validate(propertyName, value);
            }

            base.SetMember(propertyName, value);
        }

        protected virtual IEnumerable<ValidationAttribute> GetValidationAttributes(PropertyInfo propertyInfo)
        {
            var validationAttributes = new List<ValidationAttribute>();

            foreach (ValidationAttribute item in propertyInfo.GetCustomAttributes(typeof (ValidationAttribute), true))
            {
                validationAttributes.Add(item);
            }

            return validationAttributes;
        }

        protected virtual bool Validate(PropertyInfo propertyInfo, object value)
        {
            var validationAttributes = GetValidationAttributes(propertyInfo);
            var attributes = validationAttributes as IList<ValidationAttribute> ?? validationAttributes.ToList();
            if (!attributes.Any())
            {
                return true;
            }

            var validationContext = new ValidationContext(ProxiedObject, null, null);
            var results = new Collection<ValidationResult>();

            var returnValue = Validator.TryValidateValue(value, validationContext, results, attributes);

            if (returnValue)
            {
                if (validationResults.ContainsKey(propertyInfo.Name))
                {
                    validationResults.Remove(propertyInfo.Name);
                }
            }
            else
            {
                if (validationResults.ContainsKey(propertyInfo.Name))
                {
                    validationResults[propertyInfo.Name] = results;
                }
                else
                {
                    validationResults.Add(propertyInfo.Name, results);
                }
            }

            return returnValue;
        }

        protected virtual bool Validate(string propertyName, object value)
        {
            return Validate(GetPropertyInfo(propertyName), value);
        }

        #endregion

        #region constructor

        public ValidatingProxy()
        {
            ValidateOnChange = true;
        }

        public ValidatingProxy(object proxiedObject)
            : base(proxiedObject)
        {
            ValidateOnChange = true;
        }

        #endregion

        #region public methods

        public virtual bool Validate(PropertyInfo propertyInfo)
        {
            return Validate(propertyInfo, GetMember(propertyInfo.Name));
        }

        public virtual bool Validate(string propertyName)
        {
            return Validate(GetPropertyInfo(propertyName));
        }

        public virtual bool Validate()
        {
            var propertiesToValidate =
                ProxiedObject.GetType()
                    .GetProperties()
                    .Where(pi => pi.GetCustomAttributes(typeof (ValidationAttribute), true).Length > 0);

            foreach (var item in propertiesToValidate)
            {
                Validate(item);
            }

            return false;
        }

        #endregion

        #region public properties

        public bool ValidateOnChange { get; set; }

        public bool HasErrors => validationResults.Count > 0;

        #endregion
    }
}