#region Usings

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CompositeApplicationFramework.Helper;

#endregion

namespace CompositeApplicationFramework.Behaviors
{
    #region Dependencies

    

    #endregion

    public static class AutoScrollOnDragBehavior
    {
        public static readonly DependencyProperty ScrollOnDragDropProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOnDragDrop",
                typeof (bool),
                typeof (AutoScrollOnDragBehavior),
                new PropertyMetadata(false, HandleScrollOnDragDropChanged));

        public static bool GetScrollOnDragDrop(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool) element.GetValue(ScrollOnDragDropProperty);
        }

        public static void SetScrollOnDragDrop(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(ScrollOnDragDropProperty, value);
        }

        private static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = d as FrameworkElement;

            if (d == null)
            {
                Debug.Fail("Invalid type!");
            }

            Unsubscribe(container);

            if (true.Equals(e.NewValue))
            {
                Subscribe(container);
            }
        }

        private static void Subscribe(UIElement container)
        {
            container.PreviewDragOver += OnContainerPreviewDragOver;
        }

        private static void OnContainerPreviewDragOver(object sender, DragEventArgs e)
        {
            var container = sender as FrameworkElement;

            var scrollViewer = container?.GetVisualDescendent<ScrollViewer>();

            if (scrollViewer == null)
            {
                return;
            }

            const double tolerance = 60;
            var verticalPos = e.GetPosition(container).Y;
            const double offset = 20;

            if (verticalPos < tolerance) // Top of visible list? 
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset); //Scroll up. 
            }
            else if (verticalPos > container.ActualHeight - tolerance) //Bottom of visible list? 
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset); //Scroll down.     
            }
        }

        private static void Unsubscribe(UIElement container)
        {
            container.PreviewDragOver -= OnContainerPreviewDragOver;
        }
    }
}