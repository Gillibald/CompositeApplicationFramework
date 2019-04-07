#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    [Flags]
    public enum DropPosition
    {
        Before = 0,

        After = 1,

        Inside = 2
    }
}