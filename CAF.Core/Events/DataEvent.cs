#region Usings



#endregion

namespace CompositeApplicationFramework.Events
{
    /// <summary>
    ///     Process event
    /// </summary>
    public class DataEvent : PubSubEvent<DataEventArgs>
    {
    }

    /// <summary>
    ///    Process event with typed identifier
    /// </summary>
    public class DataEvent<TKey> : PubSubEvent<DataEventArgs<TKey>>
    {
    }


    /// <summary>
    ///    Typed Process event with typed identifier
    /// </summary>
    public class DataEvent<T, TKey> : PubSubEvent<DataEventArgs<T, TKey>>
    {
    }
}