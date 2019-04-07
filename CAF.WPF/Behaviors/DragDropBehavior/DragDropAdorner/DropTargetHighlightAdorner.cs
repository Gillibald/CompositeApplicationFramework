#region Usings

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior.DragDropAdorner
{
    #region Dependencies

    

    #endregion

    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        public DropTargetHighlightAdorner(UIElement pAdornedElement)
            : base(pAdornedElement)
        {
        }

        protected override void OnRender(DrawingContext pDrawingContext)
        {
            var lVisualTargetItem = DropInfo.VisualTargetItem;
            if (lVisualTargetItem == null) return;
            var lRect = Rect.Empty;

            var lTreeViewItem = lVisualTargetItem as TreeViewItem;
            if (lTreeViewItem != null && VisualTreeHelper.GetChildrenCount(lTreeViewItem) > 0)
            {
                var lGrid = VisualTreeHelper.GetChild(lTreeViewItem, 0) as Grid;
                if (lGrid != null)
                {
                    var lDescendant = VisualTreeHelper.GetDescendantBounds(lTreeViewItem);
                    lRect = new Rect(
                        lTreeViewItem.TranslatePoint(new Point(), AdornedElement),
                        new Size(lDescendant.Width + 4, lGrid.RowDefinitions[0].ActualHeight));
                }
            }
            if (lRect.IsEmpty)
            {
                lRect = new Rect(
                    lVisualTargetItem.TranslatePoint(new Point(), AdornedElement),
                    VisualTreeHelper.GetDescendantBounds(lVisualTargetItem).Size);
            }
            pDrawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), lRect, 2, 2);
        }
    }
}