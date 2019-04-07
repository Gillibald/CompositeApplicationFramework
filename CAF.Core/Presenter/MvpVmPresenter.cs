#region Usings

using System;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Interfaces;
using Unity;

#endregion

namespace CompositeApplicationFramework.Presenter
{
    /// <summary>
    ///     Presenter with View/ViewModel types
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    [Region]
    public class MvpVmPresenter<TView, TViewModel> : PresenterBase<TViewModel> where TViewModel : class, IViewModel
    {
        /// <summary>
        ///     View
        /// </summary>
        public new TView View
        {
            get => (TView)base.View;
            private set => base.View = value;
        }

        [Dependency]
        public IPlatform Platform { get; set; }

        /// <summary>
        ///     Called when container is set
        /// </summary>
        protected override void OnContainerSet()
        {
            base.OnContainerSet();

            try
            {
                View = Container.Resolve<TView>();
                Platform.SetBindingContext(View, ViewModel);
                Id = ViewModel.Id;
            }
            catch (Exception ex)
            {
                Throw<Exception>(ex, "Could not instantiate presenter's view", GetType().Name);
            }
        }

        /// <summary>
        ///     Called by base dispose if not already disposed
        /// </summary>
        protected override void DisposeHandler()
        {
            base.DisposeHandler();

            View = default(TView);
        }
    }
}