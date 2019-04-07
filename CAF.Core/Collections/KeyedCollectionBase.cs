#region Usings

using System;
using System.Collections.ObjectModel;

#endregion

namespace CompositeApplicationFramework.Collections
{
    public class KeyedCollectionBase<TKey, TValue> : KeyedCollection<TKey, TValue>
    {
        private readonly Func<TValue, TKey> _getKeyForItemDelegate;

        protected KeyedCollectionBase(Func<TValue, TKey> getKeyForItemDelegate)
        {
            _getKeyForItemDelegate = getKeyForItemDelegate;
        }

        protected override TKey GetKeyForItem(TValue item)
        {
            return _getKeyForItemDelegate(item);
        }
    }
}