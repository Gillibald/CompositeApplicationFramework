#region Usings

using System;
using System.Windows;

#endregion

namespace CompositeApplicationFramework.Utility
{
    #region Dependencies

    

    #endregion

    public class Template : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof (Type),
            typeof (Template));

        public static readonly DependencyProperty DataTemplateProperty = DependencyProperty.Register(
            "DataTemplate",
            typeof (DataTemplate),
            typeof (Template));

        public Type Value
        {
            get => (Type) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public DataTemplate DataTemplate
        {
            get => (DataTemplate) GetValue(DataTemplateProperty);
            set => SetValue(DataTemplateProperty, value);
        }
    }
}