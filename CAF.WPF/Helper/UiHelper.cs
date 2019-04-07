#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#endregion

namespace CompositeApplicationFramework.Helper
{
    public static class UiHelper
    {
        public enum RelativeVerticalMousePosition
        {
            Middle,

            Top,

            Bottom
        }

        public static RelativeVerticalMousePosition GetRelativeVerticalMousePosition(FrameworkElement elm, Point pt)
        {
            if (pt.Y > 0.0 && pt.Y < 25)
            {
                return RelativeVerticalMousePosition.Top;
            }
            if (pt.Y > elm.ActualHeight - 25 && pt.Y < elm.ActualHeight)
            {
                return RelativeVerticalMousePosition.Top;
            }
            return RelativeVerticalMousePosition.Middle;
        }

        public static object GetItemFromPointInItemsControl(ItemsControl parent, Point p)
        {
            var element = parent.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (Equals(element, parent))
                {
                    return null;
                }

                var data = parent.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        public static UIElement GetItemContainerFromPointInItemsControl(ItemsControl parent, Point p)
        {
            var element = parent.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (Equals(element, parent))
                {
                    return null;
                }

                var data = parent.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return element;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        public static T GetVisualAncestor<T>(this DependencyObject d) where T : class
        {
            var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());

            while (item != null)
            {
                var itemAsT = item as T;
                if (itemAsT != null)
                {
                    return itemAsT;
                }
                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        public static DependencyObject GetVisualAncestor(this DependencyObject d, Type type)
        {
            var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());

            while (item != null)
            {
                if (item.GetType() == type)
                {
                    return item;
                }
                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        public static T GetVisualDescendent<T>(this DependencyObject d) where T : DependencyObject
        {
            return d.GetVisualDescendents<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetVisualDescendents<T>(this DependencyObject d) where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var n = 0; n < childCount; n++)
            {
                var child = VisualTreeHelper.GetChild(d, n);

                var descendents = child as T;
                if (descendents != null)
                {
                    yield return descendents;
                }

                foreach (var match in GetVisualDescendents<T>(child))
                {
                    yield return match;
                }
            }
        }

        private static DependencyObject FindVisualTreeRoot(this DependencyObject d)
        {
            var current = d;
            var result = d;

            while (current != null)
            {
                result = current;
                if (current is Visual || current is Visual3D)
                {
                    break;
                }
                // If we're in Logical Land then we must walk 
                // up the logical tree until we find a 
                // Visual/Visual3D to get us back to Visual Land.
                current = LogicalTreeHelper.GetParent(current);
            }

            return result;
        }

        public static T ParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                //get parent item
                var parentObject = VisualTreeHelper.GetParent(child);

                //we've reached the end of the tree
                if (parentObject == null)
                {
                    return null;
                }

                //check if the parent matches the type we're looking for
                var parent = parentObject as T;
                if (parent != null)
                {
                    return parent;
                }
                child = parentObject;
            }
        }
    }
}