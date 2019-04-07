#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CompositeApplicationFramework.Properties;

#endregion

namespace CompositeApplicationFramework.Helper
{
    public static class FrameworkElementHelper
    {
        public static void DelayedFocus(this UIElement pElement)
        {
            pElement.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        pElement.Focusable = true;
                        pElement.Focus();
                        Keyboard.Focus(pElement);
                    }),
                DispatcherPriority.Render);
        }

        public static Image ToImage(this FrameworkElement element, Transform transform)
        {
            var elementAsImage = new Image();

            var bitmap = new RenderTargetBitmap(
                (int) element.ActualWidth,
                (int) element.ActualHeight,
                96,
                96,
                PixelFormats.Pbgra32);

            var sourceBrush = new VisualBrush(element) {Stretch = Stretch.None};
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.DrawRectangle(
                    sourceBrush,
                    null,
                    new Rect(new Point(0, 0), new Point(element.ActualWidth, element.ActualHeight)));
            }

            bitmap.Render(drawingVisual);

            elementAsImage.Source = bitmap;
            elementAsImage.Opacity = 0.85;
            return elementAsImage;
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parentObject = VisualTreeHelper.GetParent(child);

                if (parentObject == null)
                {
                    return null;
                }

                var parent = parentObject as T;
                if (parent != null)
                {
                    return parent;
                }
                child = parentObject;
            }
        }

        public static ItemsControl FindItemsConrolParent(this FrameworkElement target)
        {
            var result = target.Parent as ItemsControl;
            if (result != null)
            {
                return result;
            }
            result = ItemsControl.ItemsControlFromItemContainer(target);
            return result ?? FindVisualParent<ItemsControl>(target);
        }

        public static T FindVisualParent<T>(FrameworkElement target) where T : FrameworkElement
        {
            while (true)
            {
                if (target == null)
                {
                    return null;
                }
                var visParent = VisualTreeHelper.GetParent(target);
                var result = visParent as T;
                if (result != null)
                {
                    return result;
                }
                target = visParent as FrameworkElement;
            }
        }

        public static T GetTemplateChild<T>(this Control target, string templtePartName) where T : FrameworkElement
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
            return frameworkElement?.FindName(templtePartName) as T;
        }
    }
}