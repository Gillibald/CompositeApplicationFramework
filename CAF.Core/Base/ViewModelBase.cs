#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Extensions;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Types;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Base
{
    public abstract class ViewModelBase : ClassBase, IViewModel
    {
        private readonly TaskFactory _taskFactory = CreateTaskFactory();
        private IViewModel _parentViewModel;

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the parent view model.
        /// </summary>
        /// <value>
        /// The parent view model.
        /// </value>
        public IViewModel ParentViewModel
        {
            get => _parentViewModel;
            set
            {
                var oldViewModel = _parentViewModel;
                _parentViewModel = value;
                RaisePropertyChanged(() => ParentViewModel);
                OnParentViewModelChanged(oldViewModel, value);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     IViewModel.Initialize view model
        /// </summary>
        async Task IViewModel.InitializeAsync()
        {
            if (IsInitialized)
            {
                return;
            }

            await InitializeAsync();

            IsInitialized = true;
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

        /// <inheritdoc />
        /// <summary>
        /// A hook into data that ViewModels want to share
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual Dictionary<string, object> GetViewModelData(object sender, EventArgs e)
        {
            return new Dictionary<string, object>();
        }

        private static TaskFactory CreateTaskFactory()
        {
            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }
            return new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        protected void Invoke([NotNull] Action action)
        {
            _taskFactory.StartNew(action).Wait();
        }

        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="action">The action.</param>
        protected Task BeginInvoke([NotNull] Action action)
        {
            return _taskFactory.StartNew(action);
        }

        /// <summary>
        /// Called when [parent view model changed].
        /// </summary>
        /// <param name="oldViewModel">The old view model.</param>
        /// <param name="newViewModel">The new view model.</param>
        protected virtual void OnParentViewModelChanged(IViewModel oldViewModel, IViewModel newViewModel)
        {
        }

        /// <summary>
        ///     Initialize view model override
        /// </summary>
#pragma warning disable 1998
#pragma warning disable 1998
        protected virtual async Task InitializeAsync()
#pragma warning restore 1998
#pragma warning restore 1998
        {
        
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the p property.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pPropertyExpression">The p property expression.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> pPropertyExpression)
        {
            var propertyName = pPropertyExpression.GetPropertyName();
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class ViewModelBase<T> : ViewModelBase, IViewModel<T>
    {
        private T _model;

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
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

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        /// <param name="oldModel">The old model.</param>
        /// <param name="newModel">The new value.</param>
        protected virtual void OnModelChanged(T oldModel, T newModel)
        {
        }
    }
}