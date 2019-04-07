#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IViewModelCollection<out TSource, out TTarget> : IEnumerable<TTarget>, IList, INotifyCollectionChanged
    {
        IEnumerable<TSource> Source { get; }
    }
}