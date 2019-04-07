using System.Windows;
using System.Windows.Media;

namespace CompositeApplicationFramework.Extensions
{
    public static class DependencyObjectExtension
    {
        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parentObject = VisualTreeHelper.GetParent(child);

                switch (parentObject)
                {
                    case null:
                        return null;
                    case T parent:
                        return parent;
                }

                child = parentObject;
            }
        }

        public static T FindVisualChild<T>(this DependencyObject element) where T : class
        {
            if (element == null)
            {
                return default(T);
            }

            if (element.GetType() == typeof(T))
            {
                return element as T;
            }

            T foundElement = null;

            (element as FrameworkElement)?.ApplyTemplate();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;

                foundElement = visual.FindVisualChild<T>();

                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }
    }
}