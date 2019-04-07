#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CompositeApplicationFramework.Helper;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    #region Dependencies



    #endregion

    public static class ItemsControlExtensions
    {
        public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
        {
            switch (itemsControl)
            {
                case MultiSelector _:
                    // The CanSelectMultipleItems property is protected. Use reflection to
                    // get its value anyway.
                    var propertyCanSelectMultipleItems = itemsControl.GetType()
                        .GetProperty("CanSelectMultipleItems", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (propertyCanSelectMultipleItems == null) return false;
                    return (bool)propertyCanSelectMultipleItems.GetValue(itemsControl, null);
                case ListBox box:
                    return box.SelectionMode != SelectionMode.Single;
            }

            return false;
        }

        public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
        {
            var itemType = GetItemContainerType(itemsControl);

            if (itemType != null)
            {
                return (UIElement)child.GetVisualAncestor(itemType);
            }

            return null;
        }

        public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
        {
            var inputElement = itemsControl.InputHitTest(position);

            if (inputElement is UIElement uiElement)
            {
                return GetItemContainer(itemsControl, uiElement);
            }

            return null;
        }

        public static UIElement GetItemContainerAt(
            this ItemsControl itemsControl,
            Point position,
            Orientation searchDirection)
        {
            var itemContainerType = GetItemContainerType(itemsControl);

            if (itemContainerType == null) return null;

            Geometry hitTestGeometry;

            if (typeof(TreeViewItem).IsAssignableFrom(itemContainerType))
            {
                hitTestGeometry = new LineGeometry(
                    new Point(0, position.Y),
                    new Point(itemsControl.RenderSize.Width, position.Y));
            }
            else
            {
                switch (searchDirection)
                {
                    case Orientation.Horizontal:
                        hitTestGeometry = new LineGeometry(
                            new Point(0, position.Y),
                            new Point(itemsControl.RenderSize.Width, position.Y));
                        break;
                    case Orientation.Vertical:
                        hitTestGeometry = new LineGeometry(
                            new Point(position.X, 0),
                            new Point(position.X, itemsControl.RenderSize.Height));
                        break;
                    default:
                        throw new ArgumentException("Invalid value for searchDirection");
                }
            }

            var hits = new List<DependencyObject>();

            VisualTreeHelper.HitTest(
                itemsControl,
                null,
                result =>
                {
                    var itemContainer = result.VisualHit.GetVisualAncestor(itemContainerType);
                    if (itemContainer != null && !hits.Contains(itemContainer)
                        && ((UIElement)itemContainer).IsVisible)
                    {
                        hits.Add(itemContainer);
                    }
                    return HitTestResultBehavior.Continue;
                },
                new GeometryHitTestParameters(hitTestGeometry));

            return GetClosest(itemsControl, hits, position, searchDirection);
        }

        public static Type GetItemContainerType(this ItemsControl itemsControl)
        {
            // There is no safe way to get the item container type for an ItemsControl. 
            // First hard-code the types for the common ItemsControls.
            if (itemsControl.GetType().IsAssignableFrom(typeof(ListBox)))
            {
                return typeof(ListBoxItem);
            }
            if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
            {
                return typeof(TreeViewItem);
            }
            if (itemsControl.GetType().IsAssignableFrom(typeof(ListView)))
            {
                return typeof(ListViewItem);
            }

            // Otherwise look for the control's ItemsPresenter, get it's child panel and the first 
            // child of that *should* be an item container.
            //
            // If the control currently has no items, we're out of luck.
            if (itemsControl.Items.Count <= 0)
            {
                return null;
            }
            var itemsPresenters = itemsControl.GetVisualDescendents<ItemsPresenter>();

            return (from itemsPresenter in itemsPresenters
                    select VisualTreeHelper.GetChild(itemsPresenter, 0)
                into panel
                    select VisualTreeHelper.GetChildrenCount(panel) > 0 ? VisualTreeHelper.GetChild(panel, 0) : null
                into itemContainer
                    where
                        itemContainer != null
                        && itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1
                    select itemContainer.GetType()).FirstOrDefault();
        }

        public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
        {
            var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>();

            if (itemsPresenter == null) return Orientation.Vertical;
            var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
            var orientationProperty = itemsPanel.GetType().GetProperty("Orientation", typeof(Orientation));

            if (orientationProperty != null)
            {
                return (Orientation)orientationProperty.GetValue(itemsPanel, null);
            }

            // Make a guess!
            return Orientation.Vertical;
        }

        public static FlowDirection GetItemsPanelFlowDirection(this ItemsControl itemsControl)
        {
            var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>();

            if (itemsPresenter == null)
            {
                return FlowDirection.LeftToRight;
            }
            var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
            var flowDirectionProperty = itemsPanel.GetType().GetProperty("FlowDirection", typeof(FlowDirection));

            if (flowDirectionProperty != null)
            {
                return (FlowDirection)flowDirectionProperty.GetValue(itemsPanel, null);
            }

            // Make a guess!
            return FlowDirection.LeftToRight;
        }

        public static void SetSelectedItem(this ItemsControl itemsControl, object item)
        {
            if (itemsControl is MultiSelector selector)
            {
                selector.SelectedItem = null;
                selector.SelectedItem = item;
            }
            else if (itemsControl is ListBox)
            {
                ((ListBox)itemsControl).SelectedItem = null;
                ((ListBox)itemsControl).SelectedItem = item;
            }
            else if (itemsControl is TreeView)
            {
                // TODO: Select the TreeViewItem
                //((TreeView)itemsControl)
            }
            else
            {
                if (!(itemsControl is Selector control)) return;
                control.SelectedItem = null;
                control.SelectedItem = item;
            }
        }

        public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
        {
            if (itemsControl.GetType().IsAssignableFrom(typeof(MultiSelector)))
            {
                return ((MultiSelector)itemsControl).SelectedItems;
            }
            if (itemsControl.GetType().IsAssignableFrom(typeof(ListBox)))
            {
                var listBox = (ListBox)itemsControl;

                if (listBox.SelectionMode == SelectionMode.Single)
                {
                    return Enumerable.Repeat(listBox.SelectedItem, 1);
                }
                return listBox.SelectedItems;
            }
            if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
            {
                return Enumerable.Repeat(((TreeView)itemsControl).SelectedItem, 1);
            }
            return itemsControl.GetType().IsAssignableFrom(typeof(Selector))
                ? Enumerable.Repeat(((Selector)itemsControl).SelectedItem, 1)
                : Enumerable.Empty<object>();
        }

        public static bool GetItemSelected(this ItemsControl itemsControl, object item)
        {
            if (itemsControl is MultiSelector control)
            {
                return control.SelectedItems.Contains(item);
            }

            if (itemsControl is ListBox box)
            {
                return box.SelectedItems.Contains(item);
            }

            if (itemsControl is TreeView view)
            {
                return view.SelectedItem == item;
            }

            if (itemsControl is Selector selector)
            {
                return selector.SelectedItem == item;
            }
            return false;
        }

        public static void SetItemSelected(this ItemsControl itemsControl, object item, bool value)
        {
            if (itemsControl is MultiSelector control)
            {
                var multiSelector = control;

                if (value)
                {
                    if (multiSelector.CanSelectMultipleItems())
                    {
                        multiSelector.SelectedItems.Add(item);
                    }
                    else
                    {
                        multiSelector.SelectedItem = item;
                    }
                }
                else
                {
                    multiSelector.SelectedItems.Remove(item);
                }
            }
            else
            {
                if (!(itemsControl is ListBox box))
                {
                    return;
                }
                var listBox = box;

                if (value)
                {
                    if (listBox.SelectionMode != SelectionMode.Single)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                    else
                    {
                        listBox.SelectedItem = item;
                    }
                }
                else
                {
                    listBox.SelectedItems.Remove(item);
                }
            }
        }

        private static UIElement GetClosest(
            ItemsControl itemsControl,
            IEnumerable<DependencyObject> items,
            Point position,
            Orientation searchDirection)
        {
            //Console.WriteLine("GetClosest - {0}", itemsControl.ToString());
            UIElement closest = null;
            var closestDistance = double.MaxValue;

            foreach (var i in items)
            {
                if (!(i is UIElement uiElement)) continue;

                var p = uiElement.TransformToAncestor(itemsControl).Transform(new Point(0, 0));

                var distance = double.MaxValue;

                if (itemsControl is TreeView)
                {
                    var xDiff = position.X - p.X;
                    var yDiff = position.Y - p.Y;
                    var hyp = Math.Sqrt(Math.Pow(xDiff, 2d) + Math.Pow(yDiff, 2d));
                    distance = Math.Abs(hyp);
                }
                else
                {
                    switch (searchDirection)
                    {
                        case Orientation.Horizontal:
                            distance = Math.Abs(position.X - p.X);
                            break;
                        case Orientation.Vertical:
                            distance = Math.Abs(position.Y - p.Y);
                            break;
                    }
                }

                if (!(distance < closestDistance))
                {
                    continue;
                }
                closest = uiElement;
                closestDistance = distance;
            }

            return closest;
        }
    }
}