#region Usings

using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Model;
using CompositeApplicationFramework.Types;

#endregion

namespace CompositeApplicationFramework.Presenter
{
    /// <summary>
    ///     Status bar presenter
    ///     + View = StatusBarView
    ///     + ViewModel = implementation of IShellViewModel
    /// </summary>
    public class StatusBarPresenter<TView, TViewModel> : MvpVmPresenter<TView, TViewModel> where TViewModel : class, IStatusBarViewModel
    {
        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        protected override void ProcessEventLocalHandler(ProcessEventArgs e)
        {
            #region Do we handle event?

            var isHandled = (e.ProcessType == ProcessType.StatusBarMessage || e.ProcessType == ProcessType.MenuSelected);
            e.IsHandled = isHandled;
            base.ProcessEventLocalHandler(e); // Log activity
            if (!isHandled)
            {
                return;
            }

            #endregion

            // Local is processed as well as global
            ProcessEventGlobalHandler(e);
        }

        /// <summary>
        ///     Status bar wants to be kept informed of status bar messages
        ///     so we can update status bar message
        /// </summary>
        /// <param name="e"></param>
        protected override void ProcessEventGlobalHandler(ProcessEventArgs e)
        {
            #region Do we handle event?

            var isHandled = (e.ProcessType == ProcessType.StatusBarMessage || e.ProcessType == ProcessType.MenuSelected);
            e.IsHandled = isHandled;
            base.ProcessEventGlobalHandler(e); // Log activity
            if (!isHandled)
            {
                return;
            }

            #endregion

            if (e.ProcessType == ProcessType.MenuSelected)
            {
                ViewModel.StatusBarMessage = $"Menu {e.GetData<MenuEntry>().Name} selected";
                return;
            }

            var args = e as MessageEventArgs;

            if (args == null)
            {
                return;
            }

            ViewModel.StatusBarMessage = args.Message;

            Logger.Info("\t\t=> StatusBarMessage = [{0}]", args.Message);
        }
    }
}