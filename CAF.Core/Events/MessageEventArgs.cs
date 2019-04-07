#region Usings

using CompositeApplicationFramework.Types;

#endregion

namespace CompositeApplicationFramework.Events
{
    /// <summary>
    ///     Message event argument
    /// </summary>
    public class MessageEventArgs : ProcessEventArgs
    {
        /// <summary>
        ///     Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Message type
        /// </summary>
        public MessageType MessageType { get; set; }
    }
}