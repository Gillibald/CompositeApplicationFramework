using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CompositeApplicationFramework.Extensions
{
    public static class FrameworkElementExtension
    {  
        public static Image ToImage(this FrameworkElement element, Transform transform)
        {
            var elementAsImage = new Image();

            var bitmap = new RenderTargetBitmap(
                (int)element.ActualWidth,
                (int)element.ActualHeight,
                96,
                96,
                PixelFormats.Pbgra32);

            var sourceBrush = new VisualBrush(element) { Stretch = Stretch.None };
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

        public static ItemsControl FindItemsConrolParent(this FrameworkElement target)
        {
            if (target.Parent is ItemsControl result)
            {
                return result;
            }
            result = ItemsControl.ItemsControlFromItemContainer(target);
            return result ?? target.FindVisualParent<ItemsControl>();
        }   
       
    }
}
