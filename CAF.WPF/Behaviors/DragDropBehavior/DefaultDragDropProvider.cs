using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CompositeApplicationFramework.Extensions;

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    public class DefaultDragDropProvider : IDragDropProvider
    {
        public IDragInfo GetDragInfo(object sender, MouseButtonEventArgs e)
        {
            var dragInfo = DragInfo.CreateDragInfo(sender, e);

            var itemsControl = sender as ItemsControl;

            if (itemsControl != null)
            {
                var lSourceItem = e.OriginalSource as UIElement;
                // If we can't cast object as a UIElement it might be a FrameworkContentElement, if so try and use its parent.
                if (lSourceItem == null && e.OriginalSource is FrameworkContentElement)
                {
                    lSourceItem = ((FrameworkContentElement)e.OriginalSource).Parent as UIElement;
                }
                UIElement lItem = null;
                if (lSourceItem != null)
                {
                    lItem = itemsControl.GetItemContainer(lSourceItem);
                }
                if (lItem == null)
                {
                    lItem = itemsControl.GetItemContainerAt(
                        e.GetPosition(itemsControl),
                        itemsControl.GetItemsPanelOrientation());
                }

                if (lItem != null)
                {
                    // Remember the relative position of the item being dragged
                    dragInfo.PositionInDraggedItem = e.GetPosition(lItem);

                    var lItemParent = ItemsControl.ItemsControlFromItemContainer(lItem);

                    if (lItemParent != null)
                    {
                        dragInfo.SourceCollection = lItemParent.ItemsSource ?? lItemParent.Items;
                        dragInfo.SourceItem = lItemParent.ItemContainerGenerator.ItemFromContainer(lItem);
                    }
                    dragInfo.SourceItems = itemsControl.GetSelectedItems();

                    // Some controls (I'm looking at you TreeView!) haven't updated their
                    // SelectedItem by this point. Check to see if there 1 or less item in 
                    // the SourceItems collection, and if so, override the control's 
                    // SelectedItems with the clicked item.
                    if (dragInfo.SourceItems.Cast<object>().Count() <= 1)
                    {
                        dragInfo.SourceItems = Enumerable.Repeat(dragInfo.SourceItem, 1);
                    }

                    dragInfo.VisualSourceItem = lItem;
                }
                else
                {
                    dragInfo.SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                }
            }
            else
            {
                var element = sender as UIElement;
                if (element != null)
                {
                    dragInfo.PositionInDraggedItem = e.GetPosition(element);
                }
            }

            if (dragInfo.SourceItems == null)
            {
                dragInfo.SourceItems = Enumerable.Empty<object>();
            }

            return dragInfo;
        }

        //public FrameworkElement GetContainerForItem(FrameworkElement element)
        //{
        //    if (element == null)
        //    {
        //        return null;
        //    }

        //    var lItem = element.GetVisualAncestor<ListBoxItem>();

        //    return lItem;
        //}

        //public IList GetList(ItemsControl itemsControl)
        //{
        //    if (itemsControl.ItemsSource is ICollectionView)
        //    {
        //        var lView = itemsControl.ItemsSource as ICollectionView;

        //        return lView.SourceCollection as IList;
        //    }

        //    return itemsControl.ItemsSource as IList;
        //}
    }
}