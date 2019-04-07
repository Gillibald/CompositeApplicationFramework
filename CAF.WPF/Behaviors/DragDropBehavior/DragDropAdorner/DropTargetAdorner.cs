#region Usings

using System;
using System.Windows;
using System.Windows.Documents;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior.DragDropAdorner
{
    #region Dependencies

    

    #endregion

    public abstract class DropTargetAdorner : Adorner
    {
        private readonly AdornerLayer _adornerLayer;

        protected DropTargetAdorner(UIElement pAdornedElement)
            : base(pAdornedElement)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(pAdornedElement);
            _adornerLayer.Add(this);
            IsHitTestVisible = false;
        }

        public DropInfo DropInfo { get; set; }

        public void Detatch()
        {
            _adornerLayer.Remove(this);
        }

        internal static DropTargetAdorner Create(Type pType, UIElement pAdornedElement)
        {
            if (!typeof (DropTargetAdorner).IsAssignableFrom(pType))
            {
                throw new InvalidOperationException(
                    "The requested adorner class does not derive from DropTargetAdorner.");
            }
            var constructorInfo = pType.GetConstructor(new[] {typeof (UIElement)});
            if (constructorInfo != null)
            {
                return (DropTargetAdorner) constructorInfo.Invoke(new object[] {pAdornedElement});
            }
            //Todo: default bestimmen
            return new DropTargetInsertionAdorner(pAdornedElement);
        }
    }
}