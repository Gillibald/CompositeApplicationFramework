#region Usings

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CompositeApplicationFramework.Behaviors.DragDropBehavior.DragDropAdorner;
using CompositeApplicationFramework.Extensions;
using CompositeApplicationFramework.Helper;
using CompositeApplicationFramework.Properties;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    /// <summary>
    ///     Holds information about a the target of a drag drop operation.
    /// </summary>
    /// <remarks>
    ///     The <see cref="DropInfo" /> class holds all of the framework's information about the current
    ///     target of a drag. It is used by <see cref="IDropTarget.DragOver" /> method to determine whether
    ///     the current drop target is valid, and by <see cref="IDropTarget.Drop" /> to perform the drop.
    /// </remarks>
    public class DropInfo : IDropInfo
    {
        /// <summary>
        ///     Initializes a new instance of the DropInfo class.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the drag event.
        /// </param>
        /// <param name="e">
        ///     The drag event.
        /// </param>
        /// <param name="dragInfo">
        ///     Information about the source of the drag, if the drag came from within the framework.
        /// </param>
        public DropInfo(object sender, DragEventArgs e, IDragInfo dragInfo)
        {
            var lDataFormat = DragDropManager.DataFormat.Name;
            Data = (e.Data.GetDataPresent(lDataFormat)) ? e.Data.GetData(lDataFormat) : e.Data;
            DragInfo = dragInfo;
            KeyStates = e.KeyStates;

            VisualTarget = sender as UIElement;
            if (VisualTarget != null)
            {
                DropPosition = e.GetPosition(VisualTarget);
            }

            var lItemsControl = sender as ItemsControl;

            if (lItemsControl == null) return;
            var lItem = lItemsControl.GetItemContainerAt(DropPosition);
            var lDirectlyOverItem = lItem != null;

            TargetGroup = FindGroup(lItemsControl, DropPosition);
            VisualTargetOrientation = lItemsControl.GetItemsPanelOrientation();
            VisualTargetFlowDirection = lItemsControl.GetItemsPanelFlowDirection();

            if (lItem == null)
            {
                lItem = lItemsControl.GetItemContainerAt(DropPosition, VisualTargetOrientation);
            }

            if (lItem != null)
            {
                var lItemParent = ItemsControl.ItemsControlFromItemContainer(lItem);

                InsertIndex = lItemParent.ItemContainerGenerator.IndexFromContainer(lItem);
                TargetCollection = lItemParent.ItemsSource ?? lItemParent.Items;

                if (lDirectlyOverItem || lItem is TreeViewItem)
                {
                    TargetItem = lItemParent.ItemContainerGenerator.ItemFromContainer(lItem);
                    VisualTargetItem = lItem;
                }

                var lItemRenderSize = lItem.RenderSize;

                if (VisualTargetOrientation == Orientation.Vertical)
                {
                    var lCurrentYPos = e.GetPosition(lItem).Y;
                    var lTargetHeight = lItemRenderSize.Height;

                    if (lCurrentYPos > lTargetHeight/2)
                    {
                        InsertIndex++;
                        InsertPosition = DragDropBehavior.DropPosition.After;
                    }
                    else
                    {
                        InsertPosition = DragDropBehavior.DropPosition.Before;
                    }

                    if (lCurrentYPos > lTargetHeight*0.25 && lCurrentYPos < lTargetHeight*0.75)
                    {
                        InsertPosition |= DragDropBehavior.DropPosition.Inside;
                    }
                }
                else
                {
                    var lCurrentXPos = e.GetPosition(lItem).X;
                    var lTargetWidth = lItemRenderSize.Width;

                    if ((VisualTargetFlowDirection == FlowDirection.RightToLeft && lCurrentXPos < lTargetWidth/2)
                        || (VisualTargetFlowDirection == FlowDirection.LeftToRight
                            && lCurrentXPos > lTargetWidth/2))
                    {
                        InsertIndex++;
                        InsertPosition = DragDropBehavior.DropPosition.After;
                    }
                    else
                    {
                        InsertPosition = DragDropBehavior.DropPosition.Before;
                    }

                    if (lCurrentXPos > lTargetWidth*0.25 && lCurrentXPos < lTargetWidth*0.75)
                    {
                        InsertPosition |= DragDropBehavior.DropPosition.Inside;
                    }
#if DEBUG
                    Console.WriteLine(
                        Resources.DropInfo_DebugInfo,
                        InsertPosition,
                        lItem,
                        InsertIndex,
                        lCurrentXPos);
#endif
                }
            }
            else
            {
                TargetCollection = lItemsControl.ItemsSource ?? lItemsControl.Items;
                InsertIndex = lItemsControl.Items.Count;
            }
        }

        /// <summary>
        ///     Gets the drag data.
        /// </summary>
        /// <remarks>
        ///     If the drag came from within the framework, this will hold:
        ///     - The dragged data if a single item was dragged.
        ///     - A typed IEnumerable if multiple items were dragged.
        /// </remarks>
        public object Data { get; }

        /// <summary>
        ///     Gets a <see cref="DragInfo" /> object holding information about the source of the drag,
        ///     if the drag came from within the framework.
        /// </summary>
        public IDragInfo DragInfo { get; }

        /// <summary>
        ///     Gets the mouse position relative to the VisualTarget
        /// </summary>
        public Point DropPosition { get; }

        /// <summary>
        ///     Gets or sets the class of drop target to display.
        /// </summary>
        /// <remarks>
        ///     The standard drop target adorner classes are held in the <see cref="DropTargetAdorners" />
        ///     class.
        /// </remarks>
        public Type DropTargetAdorner { get; set; }

        /// <summary>
        ///     Gets or sets the allowed effects for the drop.
        /// </summary>
        /// <remarks>
        ///     This must be set to a value other than <see cref="DragDropEffects.None" /> by a drop handler in order
        ///     for a drop to be possible.
        /// </remarks>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        ///     Gets the current insert position within <see cref="TargetCollection" />.
        /// </summary>
        public int InsertIndex { get; }

        /// <summary>
        ///     Gets the collection that the target ItemsControl is bound to.
        /// </summary>
        /// <remarks>
        ///     If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable TargetCollection { get; }

        /// <summary>
        ///     Gets the object that the current drop target is bound to.
        /// </summary>
        /// <remarks>
        ///     If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object TargetItem { get; }

        /// <summary>
        ///     Gets the current group target.
        /// </summary>
        /// <remarks>
        ///     If the drag is currently over an ItemsControl with groups, describes the group that
        ///     the drag is currently over.
        /// </remarks>
        public CollectionViewGroup TargetGroup { get; }

        /// <summary>
        ///     Gets the control that is the current drop target.
        /// </summary>
        public UIElement VisualTarget { get; }

        /// <summary>
        ///     Gets the item in an ItemsControl that is the current drop target.
        /// </summary>
        /// <remarks>
        ///     If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public UIElement VisualTargetItem { get; }

        /// <summary>
        ///     Gets the orientation of the current drop target.
        /// </summary>
        public Orientation VisualTargetOrientation { get; }

        /// <summary>
        ///     Gets the orientation of the current drop target.
        /// </summary>
        public FlowDirection VisualTargetFlowDirection { get; }

        /// <summary>
        ///     Gets and sets the text displayed in the DropDropEffects adorner.
        /// </summary>
        public string DestinationText { get; set; }

        /// <summary>
        ///     Gets the relative position the item will be inserted to compared to the TargetItem
        /// </summary>
        public DropPosition InsertPosition { get; }

        /// <summary>
        ///     Gets a flag enumeration indicating the current state of the SHIFT, CTRL, and ALT keys, as well as the state of the
        ///     mouse buttons.
        /// </summary>
        public DragDropKeyStates KeyStates { get; }

        private static CollectionViewGroup FindGroup(UIElement pItemsControl, Point pPosition)
        {
            var lElement = pItemsControl.InputHitTest(pPosition) as DependencyObject;

            var lGroupItem = lElement?.GetVisualAncestor<GroupItem>();

            return lGroupItem?.Content as CollectionViewGroup;
        }
    }
}