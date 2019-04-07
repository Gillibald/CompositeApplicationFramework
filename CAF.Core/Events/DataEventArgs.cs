#region Usings

using System;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Types;

#endregion

namespace CompositeApplicationFramework.Events
{
    /// <summary>
    ///     Data event args with typed Id
    /// </summary>
    public class DataEventArgs<T, TKey> : EventArgs, IId<TKey>
    {
        /// <summary>
        ///     Data event type (create, read, update, delete, list)
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        ///     Event data as/if applicable
        /// </summary>
        public EventArgs EventData { get; set; }

        /// <summary>
        ///     Exception if applicable
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        ///     Data object
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        ///     Generic data
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        ///     Gets or sets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public object Sender { get; set; }

        /// <summary>
        ///     Gets or sets the friendly message.
        /// </summary>
        /// <value>The friendly message.</value>
        public string FriendlyMessage { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public TKey Id { get; set; }
    }

    /// <summary>
    ///     Data event args with typed Id
    /// </summary>
    public class DataEventArgs<T> : DataEventArgs<T, Guid>
    {
    }

    public class DataEventArgs : DataEventArgs<object>
    {
        /// <summary>
        ///     Get data strongly typed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            return (T) Data;
        }
    }
}