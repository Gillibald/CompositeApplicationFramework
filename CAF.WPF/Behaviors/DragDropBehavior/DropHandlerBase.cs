#region Usings

using System;
using System.Windows;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    #region Dependencies

    

    #endregion

    public class DropHandlerBase : IDropTarget
    {
        protected readonly Func<object, bool> CanDropDelegate;
        protected readonly Action<object> DropDelegate;

        public DropHandlerBase(Action<object> dropDelegate, Func<object, bool> canDropDelegate)
        {
            DropDelegate = dropDelegate;
            CanDropDelegate = canDropDelegate;
        }

        public virtual void DragOver(IDropInfo pDropInfo)
        {
            if (CanDrop(pDropInfo))
            {
                pDropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data == null)
            {
                return;
            }

            DropDelegate(dropInfo.Data);
        }

        protected bool CanDrop(IDropInfo pDropInfo)
        {
            return CanDropDelegate(pDropInfo.Data);
        }
    }
}