#region Usings

using System;
using System.Security.Permissions;
using System.Windows.Threading;

#endregion

namespace CompositeApplicationFramework.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DispatcherHelper
    {
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames),
                frame);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame) frame).Continue = false;

            return null;
        }
    }
}