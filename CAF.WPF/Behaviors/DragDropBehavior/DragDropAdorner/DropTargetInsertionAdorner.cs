#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior.DragDropAdorner
{
    #region Dependencies

    

    #endregion

    public class DropTargetInsertionAdorner : DropTargetAdorner
    {
        private static readonly Pen Pen;
        private static readonly PathGeometry Triangle;

        static DropTargetInsertionAdorner()
        {
            // Create the pen and triangle in a static constructor and freeze them to improve performance.
            const int triangleSize = 3;

            Pen = new Pen(Brushes.Gray, 2);
            Pen.Freeze();

            var lFirstLine = new LineSegment(new Point(0, -triangleSize), false);
            lFirstLine.Freeze();
            var lSecondLine = new LineSegment(new Point(0, triangleSize), false);
            lSecondLine.Freeze();

            var lFigure = new PathFigure {StartPoint = new Point(triangleSize, 0)};
            lFigure.Segments.Add(lFirstLine);
            lFigure.Segments.Add(lSecondLine);
            lFigure.Freeze();

            Triangle = new PathGeometry();
            Triangle.Figures.Add(lFigure);
            Triangle.Freeze();
        }

        public DropTargetInsertionAdorner(UIElement pAdornedElement)
            : base(pAdornedElement)
        {
        }

        protected override void OnRender(DrawingContext pDrawingContext)
        {
            var lItemsControl = DropInfo.VisualTarget as ItemsControl;

            if (lItemsControl == null) return;
            // Get the position of the item at the insertion index. If the insertion point is
            // to be after the last item, then get the position of the last item and add an 
            // offset later to draw it at the end of the list.

            var lItemParent = DropInfo.VisualTargetItem != null
                ? ItemsControl.ItemsControlFromItemContainer(DropInfo.VisualTargetItem)
                : lItemsControl;

            var lIndex = Math.Min(DropInfo.InsertIndex, lItemParent.Items.Count - 1);
            var lItemContainer = lItemParent.ItemContainerGenerator.ContainerFromIndex(lIndex) as UIElement;

            if (lItemContainer == null) return;
            var lItemRect = new Rect(
                lItemContainer.TranslatePoint(new Point(), AdornedElement),
                lItemContainer.RenderSize);
            Point lPointA, lPointB;
            double lRotation = 0;

            if (DropInfo.VisualTargetOrientation == Orientation.Vertical)
            {
                if (DropInfo.InsertIndex == lItemParent.Items.Count)
                {
                    lItemRect.Y += lItemContainer.RenderSize.Height;
                }

                lPointA = new Point(lItemRect.X, lItemRect.Y);
                lPointB = new Point(lItemRect.Right, lItemRect.Y);
            }
            else
            {
                var lItemRectX = lItemRect.X;

                if (DropInfo.VisualTargetFlowDirection == FlowDirection.LeftToRight
                    && DropInfo.InsertIndex == lItemParent.Items.Count)
                {
                    lItemRectX += lItemContainer.RenderSize.Width;
                }
                else if (DropInfo.VisualTargetFlowDirection == FlowDirection.RightToLeft
                         && DropInfo.InsertIndex != lItemParent.Items.Count)
                {
                    lItemRectX += lItemContainer.RenderSize.Width;
                }

                lPointA = new Point(lItemRectX, lItemRect.Y);
                lPointB = new Point(lItemRectX, lItemRect.Bottom);
                lRotation = 90;
            }

            pDrawingContext.DrawLine(Pen, lPointA, lPointB);
            DrawTriangle(pDrawingContext, lPointA, lRotation);
            DrawTriangle(pDrawingContext, lPointB, 180 + lRotation);
        }

        private static void DrawTriangle(DrawingContext pDrawingContext, Point pOrigin, double pRotation)
        {
            pDrawingContext.PushTransform(new TranslateTransform(pOrigin.X, pOrigin.Y));
            pDrawingContext.PushTransform(new RotateTransform(pRotation));

            pDrawingContext.DrawGeometry(Pen.Brush, null, Triangle);

            pDrawingContext.Pop();
            pDrawingContext.Pop();
        }
    }
}