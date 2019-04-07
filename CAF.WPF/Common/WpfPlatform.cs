using System.Windows;
using CompositeApplicationFramework.Interfaces;

namespace CompositeApplicationFramework.Common
{
    public class WpfPlatform : IPlatform
    {
        public void SetBindingContext(object view, IViewModel context)
        {
            if (view is FrameworkElement element)
            {
                element.DataContext = context;
            }
        }

        public void SetVisibilityState(object view, bool? visibilityState)
        {
            if (!(view is FrameworkElement element)) return;

            switch (visibilityState)
            {
                case true:
                {
                    element.Visibility = Visibility.Visible;
                    break;
                }
                case false:
                {
                    element.Visibility = Visibility.Collapsed;
                    break;
                }
                case null:
                {
                    element.Visibility = Visibility.Hidden;
                    break;
                }
            }
        }
    }
}