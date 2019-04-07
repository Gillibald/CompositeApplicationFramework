#region Usings

using System;
using System.Windows;

#endregion

namespace CompositeApplicationFramework.Utility
{
    #region Dependencies

    

    #endregion

    public class TypeTemplate : TemplateBase
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type",
            typeof (Type),
            typeof (TypeTemplate));

        public Type Type
        {
            get => (Type) GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public override bool IsMatch(object pValue)
        {
            return Type.IsInstanceOfType(pValue);
        }
    }
}