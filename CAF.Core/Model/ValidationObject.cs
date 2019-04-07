#region Usings

using System;
using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Model
{
    public class ValidationObject : IValidationObject
    {
        public ValidationObject(object pAssignedObject, string pPropertyName, Func<object, bool> pValidationDelegate)
        {
            ValidationDelegate = pValidationDelegate;
            if (ValidationDelegate == null)
            {
                throw new ArgumentException("Delegate passed can't be null!");
            }

            AssignedObject = pAssignedObject;
            PropertyName = pPropertyName;
        }

        private Func<object, bool> ValidationDelegate { get; }
        public object AssignedObject { get; set; }
        public string PropertyName { get; internal set; }
        public string ErrorMessage { get; set; }

        public bool Validate()
        {
            return ValidationDelegate(AssignedObject);
        }
    }
}