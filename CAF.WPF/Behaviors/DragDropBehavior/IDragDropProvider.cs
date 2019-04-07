using System.Windows.Input;

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    public interface IDragDropProvider
    {
        IDragInfo GetDragInfo(object sender, MouseButtonEventArgs e);

        //FrameworkElement GetContainerForItem(FrameworkElement element);

        //IList GetList(FrameworkElement itemsControl);
    }
}