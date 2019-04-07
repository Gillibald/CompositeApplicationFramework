#region Usings

using System;
using System.ComponentModel;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Types;
using Unity;

#endregion

namespace CompositeApplicationFramework.Base
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Presenter with ViewModel declared
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class PresenterBase<TViewModel> : ClassBase, IPresenter
        where TViewModel : class, IViewModel
    {
        /// <summary>
        ///     View Model
        /// </summary>
        public TViewModel ViewModel { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     Presenter view
        /// </summary>
        public object View { get; protected set; }

        /// <inheritdoc />
        /// <summary>
        ///     View model reference via IViewModel - the ViewModel reference
        ///     will be strongly typed
        /// </summary>
        IViewModel IPresenter.ViewModel => ViewModel;

        public bool IsInitialized { get; private set; }

        async Task IPresenter.InitializeAsync()
        {
            if (IsInitialized)
            {
                return;
            }

            await InitializeAsync();

            IsInitialized = true;
        }

        /// <summary>
        ///     Initialize presenter and view model
        /// </summary>
        protected virtual async Task InitializeAsync()
        {
            await ViewModel.InitializeAsync();
        }

        void IPresenter.ModulesInitialized(IModuleCatalog moduleCatalog)
        {
            ModulesInitialized(moduleCatalog);
        }

        protected virtual void ModulesInitialized(IModuleCatalog moduleCatalog) { }

        /// <inheritdoc />
        /// <summary>
        ///     On container set
        /// </summary>
        protected override void OnContainerSet()
        {
            try
            {
                ViewModel = Container.Resolve<TViewModel>();

                ViewModel.PropertyChanged += ViewModelPropertyChangedHandler;
            }
            catch (Exception ex)
            {
                Throw<Exception>(ex, "Could not instantiate presenter's viewmodel", GetType().Name);
            }
        }

        /// <summary>
        ///     Called by base dispose if not already disposed
        /// </summary>
        protected override void DisposeHandler()
        {
            base.DisposeHandler();

            ViewModel.PropertyChanged -= ViewModelPropertyChangedHandler;

            ViewModel.Dispose();

            ViewModel = default(TViewModel);
        }

        /// <summary>
        ///     Publish status bar message
        /// </summary>
        /// <param name="message"></param>
        protected void PublishStatusBarMessage(string message)
        {
            var messageEventArgs = new MessageEventArgs
            {
                Id = Id,
                ProcessType = ProcessType.StatusBarMessage,
                Message = message,
                MessageType = MessageType.MessageLeft
            };

            PublishStatusBarMessage(messageEventArgs);
        }

        /// <summary>
        ///     Publish status bar message
        /// </summary>
        /// <param name="e"></param>
        protected void PublishStatusBarMessage(MessageEventArgs e)
        {
            Logger.Info(
                "PUBLISH:<ProcessEvent>(StatusBarMessage): {0}.PublishStatusBarMessage: [{1}] ({2})",
                ClassName,
                e.Message,
                Id);

            EventAggregator.GetEvent<ProcessEvent>().Publish(e);
        }

        /// <summary>
        ///     View model property changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ViewModelPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}