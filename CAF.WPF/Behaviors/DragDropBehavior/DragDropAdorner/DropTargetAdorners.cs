#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior.DragDropAdorner
{
    #region Dependencies

    

    #endregion

    public class DropTargetAdorners
    {
        public static Type Highlight => typeof (DropTargetHighlightAdorner);
        public static Type Insert => typeof (DropTargetInsertionAdorner);
    }
}