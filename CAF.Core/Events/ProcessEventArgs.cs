#region Usings

using System;
using CompositeApplicationFramework.Types;

#endregion

namespace CompositeApplicationFramework.Events
{
    /// <summary>
    ///     Process event arguments
    /// </summary>
    public class ProcessEventArgs : EventArgs
    {
        /// <summary>
        ///     Generic event args set by process as applicable
        /// </summary>
        public EventArgs EventArgs { get; set; }

        /// <summary>
        ///     Data being processed if applicable
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        ///     For process use
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        ///     Process guid
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Process is handled
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        ///     Process
        /// </summary>
        public ProcessType ProcessType { get; set; }

        /// <summary>
        ///     Return data strongly typed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            return (T) Data;
        }
    }
}