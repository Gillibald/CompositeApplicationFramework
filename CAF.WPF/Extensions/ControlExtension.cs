using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CompositeApplicationFramework.Properties;

namespace CompositeApplicationFramework.Extensions
{
    public static class ControlExtension
    {
        public static T GetTemplateChild<T>(this Control target, string templatePartName) where T : FrameworkElement
        {
            if (target == null)
            {
                throw new ArgumentNullException(
                    nameof(target),
                    Resources.FrameworkElementHelper_GetTemplateChild_Cannot_get_the_templated_child_of_a_null_object);
            }
            var childCount = VisualTreeHelper.GetChildrenCount(target);
            if (childCount == 0)
            {
                return null;
            }
            var frameworkElement = VisualTreeHelper.GetChild(target, 0) as FrameworkElement;
            return frameworkElement?.FindName(templatePartName) as T;
        }
    }
}