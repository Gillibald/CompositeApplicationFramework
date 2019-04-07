#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Events
{
    /// <summary>
    ///     Exception event args
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception _exception;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="exception"></param>
        public ErrorEventArgs(Exception exception)
        {
            _exception = exception;
        }

        /// <summary>
        ///     Get exception
        /// </summary>
        /// <returns></returns>
        public virtual Exception GetException()
        {
            return _exception;
        }
    }
}