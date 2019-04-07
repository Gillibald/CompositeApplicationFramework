#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Base
{
    /// <summary>
    ///     Disposable base class utilizing IDisposable pattern
    ///     with callback support
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        ///     Callback method
        /// </summary>
        public Action DisposeHandlerCallBack;

        /// <summary>
        ///     Not virtual - implements interface
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalizer from running
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finalizer - in case Dispose is not executed
        /// </summary>
        ~DisposableBase()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Intended to be overridden - always safe to use
        ///     since it will never be called unless applicable
        /// </summary>
        protected virtual void DisposeHandler()
        {
        }

        /// <summary>
        ///     Dispose method - always called
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing)
        {
            // Recursive calls should not attempt
            // to dispose any diposed objects
            if (_isDisposed)
            {
                return;
            }

            // Set flag
            _isDisposed = true;

            // If properly called by Dispose(), not
            // finalizer, then process diposal
            if (!isDisposing)
            {
                return;
            }
            // Provide hook into dispose 
            DisposeHandlerCallBack?.Invoke();

            DisposeHandler();
        }
    }
}