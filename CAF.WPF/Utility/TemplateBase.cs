#region Usings

using System.Windows;

#endregion

namespace CompositeApplicationFramework.Utility
{
    #region Dependencies

    

    #endregion

    public abstract class TemplateBase : DependencyObject
    {
        public static readonly DependencyProperty DataTemplateProperty = DependencyProperty.Register(
            "DataTemplate",
            typeof (DataTemplate),
            typeof (TemplateBase));

        public DataTemplate DataTemplate
        {
            get => (DataTemplate) GetValue(DataTemplateProperty);
            set => SetValue(DataTemplateProperty, value);
        }

        public abstract bool IsMatch(object pValue);
    }
}