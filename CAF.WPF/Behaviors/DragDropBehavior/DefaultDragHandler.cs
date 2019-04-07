#region Usings

using System.Linq;
using System.Windows;
using CompositeApplicationFramework.Helper;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    #region Dependencies

    

    #endregion

    public class DefaultDragHandler : IDragSource
    {
        public virtual void StartDrag(IDragInfo pDragInfo)
        {
            var lItemCount = pDragInfo.SourceItems.Cast<object>().Count();

            if (lItemCount == 1)
            {
                pDragInfo.Data = pDragInfo.SourceItems.Cast<object>().First();
            }
            else if (lItemCount > 1)
            {
                pDragInfo.Data = TypeHelper.CreateDynamicallyTypedList(pDragInfo.SourceItems);
            }

            pDragInfo.Effects = (pDragInfo.Data != null)
                ? DragDropEffects.Copy | DragDropEffects.Move
                : DragDropEffects.None;
        }

        public virtual void Dropped(IDropInfo pDropInfo)
        {
        }

        public virtual void CancelDrag(IDragInfo pDragInfo)
        {
        }
    }
}