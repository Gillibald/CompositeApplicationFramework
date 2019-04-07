#region Usings

using System.ComponentModel;
using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.ViewModel
{
    /// <summary>
    ///     MvpVm view model base
    /// </summary>
    public class MvpVmViewModel : ViewModelBase, INavigationAware
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        protected MvpVmViewModel()
        {
            PropertyChanged += PropertyChangedHandler;
        }

        /// <summary>
        ///     Executed by base if not disposed
        /// </summary>
        protected override void DisposeHandler()
        {
            base.DisposeHandler();
            PropertyChanged -= PropertyChangedHandler;
        }

        /// <summary>
        ///     Property changed handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
        }

        void INavigatedAware.OnNavigatedFrom(INavigationParameters parameters)
        {
            OnNavigatedFrom(parameters);
        }

        protected virtual void OnNavigatedFrom(INavigationParameters parameters) { }

        void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        {
            OnNavigatedTo(parameters);
        }

        protected virtual void OnNavigatedTo(INavigationParameters parameters) { }

        void INavigatingAware.OnNavigatingTo(INavigationParameters parameters)
        {
            OnNavigatingTo(parameters);
        }

        protected virtual void OnNavigatingTo(INavigationParameters parameters) { }
    }

    public class MvpVmViewModel<T> : MvpVmViewModel, IViewModel<T>
    {
        private T _model;

        public T Model
        {
            get => _model;
            set
            {
                var oldModel = _model;
                _model = value;
                RaisePropertyChanged(() => Model);
                OnModelChanged(oldModel, value);
            }
        }

        protected virtual void OnModelChanged(T oldModel, T newModel) { }
    }
}