#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using CompositeApplicationFramework.Utility;
using CompositeApplicationFramework.Utility.EasyLock;

#endregion

namespace CompositeApplicationFramework.Collections
{
    [Serializable]
    [DebuggerNonUserCode]
    public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>,
        INotifyCollectionChanged,
        ISerializable
    {
        #region GetKeyForItem

        protected override TKey GetKeyForItem(TItem item)
        {
            using (new ReadLock(_collectionLock))
            {
                return _getKeyForItemDelegate(item);
            }
        }

        #endregion

        #region AddRange

        public void AddRange(IEnumerable<TItem> items)
        {
            using (new WriteLock(_collectionLock))
            {
                _deferNotifyCollectionChanged = true;
                foreach (var item in items)
                {
                    Add(item);
                }
                _deferNotifyCollectionChanged = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #endregion

        #region SetItem

        protected override void SetItem(int index, TItem item)
        {
            using (new WriteLock(_collectionLock))
            {
                base.SetItem(index, item);
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
            }
        }

        #endregion

        #region InsertItem

        protected override void InsertItem(int index, TItem item)
        {
            using (new WriteLock(_collectionLock))
            {
                base.InsertItem(index, item);
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        #endregion

        #region ClearItems

        protected override void ClearItems()
        {
            using (new WriteLock(_collectionLock))
            {
                base.ClearItems();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #endregion

        #region RemoveItem

        protected override void RemoveItem(int index)
        {
            using (new WriteLock(_collectionLock))
            {
                var item = this[index];

                base.RemoveItem(index);
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
        }

        #endregion

        #region Move

        public void Move(int oldIndex, int newIndex)
        {
            using (new WriteLock(_collectionLock))
            {
                _deferNotifyCollectionChanged = true;

                var item = this[oldIndex];

                base.RemoveItem(oldIndex);

                base.InsertItem(newIndex, item);

                _deferNotifyCollectionChanged = false;

                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));

                _deferNotifyCollectionChanged = false;
            }
        }

        #endregion

        #region Private Member

        private readonly ReaderWriterLockSlim _collectionLock = Locks.GetLockInstance();

        private readonly Func<TItem, TKey> _getKeyForItemDelegate;

        private bool _deferNotifyCollectionChanged;

        #endregion

        #region Constructor

        public ObservableKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate, IEnumerable<TItem> items)
            : this(getKeyForItemDelegate)
        {
            AddRange(items);
        }

        public ObservableKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate)
        {
            if (getKeyForItemDelegate == null)
            {
                throw new ArgumentException("Delegate passed can't be null!");
            }
            _getKeyForItemDelegate = getKeyForItemDelegate;
        }

        protected ObservableKeyedCollection(SerializationInfo info, StreamingContext context)
        {
            var lDel = (byte[]) info.GetValue("GetKeyForItemDelegate", typeof (byte[]));
            _getKeyForItemDelegate = Serializer.Instance.DeSerializeObject<Func<TItem, TKey>>(lDel);

            var lData = (List<TItem>) info.GetValue("Collection", typeof (List<TItem>));

            foreach (var lItem in lData)
            {
                Add(lItem);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var lData = this.ToList();

            info.AddValue("Collection", lData);

            var lDel = Serializer.Instance.SerializeObject(_getKeyForItemDelegate);

            info.AddValue("GetKeyForItemDelegate", lDel);
        }

        #endregion

        #region OnCollectionChanged Member

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_deferNotifyCollectionChanged)
            {
                return;
            }
            using (new ReadLock(_collectionLock))
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, e);
                }
            }
        }

        #endregion

        #region Clone

        public ObservableKeyedCollection<TKey, TItem> Clone()
        {
            return DeepClone(this);
        }

        private static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T) formatter.Deserialize(ms);
            }
        }

        #endregion
    }
}