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
    /// <summary>
    ///     Generic class base
    /// </summary>
    public abstract class ClassBase : DisposableBase, IErrorBase, IId<Guid>
    {
        /// <summary>
        ///     Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="message"></param>
        /// <param name="para"></param>
        public void Throw<TException>(string message, params object[] para) where TException : Exception
        {
            _errorHandler.Throw<TException>(message, para);
            Logger.Error(message, para);
        }

        /// <summary>
        ///     Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="para"></param>
        public void Throw<TException>(Exception ex, string message, params object[] para) where TException : Exception
        {
            _errorHandler.Throw<TException>(ex, message, para);
            Logger.Error(message, ex);
        }

        /// <summary>
        ///     Sets error handler
        /// </summary>
        /// <param name="errorHandler"></param>
        /// <param name="isInit"></param>
        private void SetErrorHandler(IErrorBase errorHandler, bool isInit)
        {
            // cleanup old subscription
            if (!isInit)
            {
                _errorHandler.ErrorEvent -= ErrorEventHandler;
            }

            // assign and wireup
            _errorHandler = errorHandler;
            _errorHandler.ErrorEvent += ErrorEventHandler;
        }

        /// <summary>
        ///     Error handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ErrorEventHandler(object sender, ErrorEventArgs e)
        {
            // Bubble event by default
            ErrorEvent?.Invoke(sender, e);
        }

        /// <summary>
        ///     Called when container is set
        /// </summary>
        protected virtual void OnContainerSet()
        {
        }

        /// <summary>
        ///     Called when Id changes
        /// </summary>
        protected virtual void OnIdSet()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessEventGlobalHandler(ProcessEventArgs e)
        {
            var isHandled = e.IsHandled;
            if (isHandled)
            {
                Logger.Info(
                    "HANDLED:<ProcessEvent>({0}) {1}:ClassBase.ProcessEventGlobalHandler() Id={2}",
                    e.ProcessType,
                    ClassName,
                    Id,
                    "D");
            }

            e.IsHandled = false; // reset
        }

        /// <summary>
        ///     Handle only local process event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessEventLocalHandler(ProcessEventArgs e)
        {
            var isHandled = e.IsHandled;
            if (isHandled)
            {
                Logger.Info(
                    "HANDLED:<ProcessEvent>({0}) {1}:ClassBase.ProcessEventLocalHandler() Id={2}",
                    e.ProcessType,
                    GetType().Name,
                    Id,
                    "D");
            }

            e.IsHandled = false; // reset
        }

        /// <summary>
        ///     Handle process event
        /// </summary>
        /// <param name="e"></param>
        private void ProcessEventHandler(ProcessEventArgs e)
        {
            // If the process id is equal to this id then 
            // call local handler.
            if (e.Id == Id)
            {
                ProcessEventLocalHandler(e);
            }
            else
            {
                ProcessEventGlobalHandler(e);
            }
        }

        /// <summary>
        ///     Raise data event
        /// </summary>
        /// <param name="e"></param>
        protected void RaiseProcessEvent(ProcessEventArgs e)
        {
            Logger.Info("PUBLISH:<ProcessEvent>({0}): {1}.RaiseProcessEvent() Id={2}", e.ProcessType, ClassName, e.Id);

            // Common settings
            e.Id = Id;
            if (e.Sender == null)
            {
                e.Sender = GetType();
            }

            EventAggregator.GetEvent<ProcessEvent>().Publish(e);
        }

        /// <summary>
        ///     Raises the data event.
        /// </summary>
        /// <param name="e">The <see cref="DataEventArgs" /> instance containing the event data.</param>
        protected void RaiseDataEvent(DataEventArgs e)
        {
            Logger.Info("PUBLISH:<DataEvent>({0}): {1}.RaiseDataEvent() Id={2}", e.DataType, ClassName, e.Id);

            EventAggregator.GetEvent<DataEvent>().Publish(e);
        }

        /// <summary>
        ///     Sends the status bar message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The arguments.</param>
        protected void SendStatusBarMessage(string message, params object[] arguments)
        {
            var formatedMessage = string.Format(message, arguments);

            EventAggregator.GetEvent<ProcessEvent>()
                .Publish(new MessageEventArgs {ProcessType = ProcessType.StatusBarMessage, Message = formatedMessage});
        }

        /// <summary>
        ///     Gets the event args.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>DataEventArgs.</returns>
        protected DataEventArgs GetDataEventArgs(EventArgs e, DataType dataType)
        {
            // For event publishing
            var eventArgs = new DataEventArgs {EventData = e, DataType = dataType, Sender = this, Id = Id};
            return eventArgs;
        }

        /// <summary>
        ///     Raises the exception.
        /// </summary>
        /// <param name="eventArgs">
        ///     The <see cref="DataEventArgs" /> instance containing
        ///     the event data.
        /// </param>
        /// <param name="ex">The ex.</param>
        /// <param name="friendlyMessage">The friendly message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        protected bool RaiseException(DataEventArgs eventArgs, Exception ex, string friendlyMessage)
        {
            eventArgs.Exception = ex;
            eventArgs.FriendlyMessage = friendlyMessage;

            RaiseDataEvent(eventArgs);

            return false;
        }

        #region Events

        /// <inheritdoc />
        /// <summary>
        ///     Error event
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorEvent;

        #endregion

        #region Private fields

        private IUnityContainer _container;

        private IErrorBase _errorHandler;

        private Guid _id = Guid.NewGuid();

        private bool _isWiredUp;

        private SubscriptionToken _processEventToken;

        #endregion

        #region Properties

        /// <summary>
        ///     Get class name
        /// </summary>
        [Browsable(false)]
        public string ClassName => GetType().Name;

        /// <summary>
        ///     Logger
        /// </summary>
        [Browsable(false)]
        public ILoggerFacade Logger { get; private set; }

        /// <summary>
        ///     IOC Container
        /// </summary>
        [Dependency]
        [Browsable(false)]
        public IUnityContainer Container
        {
            get => _container;
            set
            {
                _container = value;

                var err = _container.Resolve<IErrorBase>();
                if (err != null)
                {
                    SetErrorHandler(err, true);
                }

                var log = _container.Resolve<ILoggerFacade>();
                if (log != null)
                {
                    Logger = log;
                }

                // Wireup aggregated events
                EventAggregator = _container.Resolve<IEventAggregator>();
                WireUpEvents();
                OnContainerSet();
            }
        }

        /// <summary>
        ///     Event aggregator
        /// </summary>
        [Browsable(false)]
        protected IEventAggregator EventAggregator { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Class guid
        /// </summary>
        public Guid Id
        {
            get => _id;
            protected set
            {
                _id = value;
                OnIdSet();
            }
        }

        #endregion

        #region Private supporting methods

        private void WireUpEvents()
        {
            if (_isWiredUp)
            {
                throw new Exception("Should only be wired up once!");
            }

            _isWiredUp = true;

            // Subscribe to process events
            Logger.Info("SUBSCRIPTION:<ProcessEvent>().Subscribe(ProcessEventHandler) in {0}.WireUpEvents()", ClassName);

            _processEventToken = EventAggregator.GetEvent<ProcessEvent>().Subscribe(ProcessEventHandler);

            WireUpEventsOverride();
        }

        protected virtual void WireUpEventsOverride()
        {
            
        }

        private void DisposeEvents()
        {
            // Unsubscribe events
            _errorHandler.ErrorEvent -= ErrorEventHandler;

            // Remove any event aggregator subscriptions
            EventAggregator.GetEvent<ProcessEvent>().Unsubscribe(_processEventToken);

            DisposeEventsOverride();

            EventAggregator = null;
        }

        protected virtual void DisposeEventsOverride()
        {
            
        }

        protected override void DisposeHandler()
        {
            base.DisposeHandler();

            DisposeEvents();

            Logger = null;

            _errorHandler = null;
        }

        #endregion
    }
}