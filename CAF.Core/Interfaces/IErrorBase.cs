#region Usings

using System;
using CompositeApplicationFramework.Events;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    /// <summary>
    ///     Error interface
    /// </summary>
    public interface IErrorBase
    {
        /// <summary>
        ///     Error event
        /// </summary>
        event EventHandler<ErrorEventArgs> ErrorEvent;

        /// <summary>
        ///     Throws specified error w/message
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="message"></param>
        /// <param name="para"></param>
        void Throw<TException>(string message, params object[] para) where TException : Exception;

        /// <summary>
        ///     Throws specified error w/message
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="para"></param>
        void Throw<TException>(Exception ex, string message, params object[] para) where TException : Exception;
    }
}