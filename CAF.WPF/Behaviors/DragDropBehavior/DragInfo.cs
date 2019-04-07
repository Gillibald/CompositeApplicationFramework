#region Usings

using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CompositeApplicationFramework.Extensions;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    /// <summary>
    ///     Holds information about a the source of a drag drop operation.
    /// </summary>
    /// <remarks>
    ///     The <see cref="DragInfo" /> class holds all of the framework's information about the source
    ///     of a drag. It is used by <see cref="IDragSource.StartDrag" /> to determine whether a drag
    ///     can start, and what the dragged data should be.
    /// </remarks>
    public class DragInfo : IDragInfo
    {
        public DragInfo()
        {
            
        }

        /// <summary>
        ///     Initializes a new instance of the DragInfo class.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the mouse event that initiated the drag.
        /// </param>
        /// <param name="e">
        ///     The mouse event that initiated the drag.
        /// </param>
        public DragInfo(object sender, MouseButtonEventArgs e)
        {
            DragStartPosition = e.GetPosition((IInputElement) sender);
            Effects = DragDropEffects.None;
            MouseButton = e.ChangedButton;
            VisualSource = sender as UIElement;

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
                    PositionInDraggedItem = e.GetPosition(lItem);

                    var lItemParent = ItemsControl.ItemsControlFromItemContainer(lItem);

                    if (lItemParent != null)
                    {
                        SourceCollection = lItemParent.ItemsSource ?? lItemParent.Items;
                        SourceItem = lItemParent.ItemContainerGenerator.ItemFromContainer(lItem);
                    }
                    SourceItems = itemsControl.GetSelectedItems();

                    // Some controls (I'm looking at you TreeView!) haven't updated their
                    // SelectedItem by this point. Check to see if there 1 or less item in 
                    // the SourceItems collection, and if so, override the control's 
                    // SelectedItems with the clicked item.
                    if (SourceItems.Cast<object>().Count() <= 1)
                    {
                        SourceItems = Enumerable.Repeat(SourceItem, 1);
                    }

                    VisualSourceItem = lItem;
                }
                else
                {
                    SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                }
            }
            else
            {
                var element = sender as UIElement;
                if (element != null)
                {
                    PositionInDraggedItem = e.GetPosition(element);
                }
            }

            if (SourceItems == null)
            {
                SourceItems = Enumerable.Empty<object>();
            }
        }

        /// <summary>
        ///     Gets or sets the drag data.
        /// </summary>
        /// <remarks>
        ///     This must be set by a drag handler in order for a drag to start.
        /// </remarks>
        public object Data { get; set; }

        /// <summary>
        ///     Gets the position of the click that initiated the drag, relative to <see cref="VisualSource" />.
        /// </summary>
        public Point DragStartPosition { get; set; }

        /// <summary>
        ///     Gets the point where the cursor was relative to the item being dragged when the drag was started.
        /// </summary>
        public Point PositionInDraggedItem { get; set; }

        /// <summary>
        ///     Gets or sets the allowed effects for the drag.
        /// </summary>
        /// <remarks>
        ///     This must be set to a value other than <see cref="DragDropEffects.None" /> by a drag handler in order
        ///     for a drag to start.
        /// </remarks>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        ///     Gets the mouse button that initiated the drag.
        /// </summary>
        public MouseButton MouseButton { get; set; }

        /// <summary>
        ///     Gets the collection that the source ItemsControl is bound to.
        /// </summary>
        /// <remarks>
        ///     If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable SourceCollection { get; set; }

        /// <summary>
        ///     Gets the object that a dragged item is bound to.
        /// </summary>
        /// <remarks>
        ///     If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object SourceItem { get; set; }

        /// <summary>
        ///     Gets a collection of objects that the selected items in an ItemsControl are bound to.
        /// </summary>
        /// <remarks>
        ///     If the control that initated the drag is unbound or not an ItemsControl, this will be empty.
        /// </remarks>
        public IEnumerable SourceItems { get; set; }

        /// <summary>
        ///     Gets the control that initiated the drag.
        /// </summary>
        public UIElement VisualSource { get; set; }

        /// <summary>
        ///     Gets the item in an ItemsControl that started the drag.
        /// </summary>
        /// <remarks>
        ///     If the control that initiated the drag is an ItemsControl, this property will hold the item
        ///     container of the clicked item. For example, if <see cref="VisualSource" /> is a telerik:RadListBox this
        ///     will hold a ListBoxItem.
        /// </remarks>
        public UIElement VisualSourceItem { get; set; }

        /// <summary>
        ///     Gets the <see cref="IDataObject" /> which is used by the drag and drop operation. Set it to
        ///     a custom instance if custom drag and drop behavior is needed.
        /// </summary>
        public IDataObject DataObject { get; set; }

        public static DragInfo CreateDragInfo(object sender, MouseButtonEventArgs e)
        {
            var dragInfo = new DragInfo
            {
                DragStartPosition = e.GetPosition((IInputElement) sender),
                Effects = DragDropEffects.None,
                MouseButton = e.ChangedButton,
                VisualSource = sender as UIElement
            };

            return dragInfo;
        }
    }
}