#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CompositeApplicationFramework.Extensions;

#endregion

namespace CompositeApplicationFramework.Collections
{
    using JetBrains.Annotations;

    public class TransformedCollection<TSource, TTarget> : ReadOnlyCollection<TTarget>,
        INotifyCollectionChanged,
        IDisposable
    {
        #region Constructors

        public TransformedCollection(
            [NotNull] IEnumerable<TSource> sourceCollection,
            [NotNull] Func<TSource, TTarget> setup,
            Action<TTarget> teardown = null)
            : base(GetTransformedItems(sourceCollection, setup))
        {
            _setup = setup;
            _teardown = teardown;

            Source = sourceCollection;
            var notifyCollectionChanged = Source as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += OnSourceCollectionChanged;
            }
        }

        private static IList<TTarget> GetTransformedItems(IEnumerable<TSource> source, Func<TSource, TTarget> setup)
        {
            return source.Select(setup).ToList();
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Private Methods

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (_teardown != null)
                {
                    foreach (var target in Items)
                    {
                        _teardown(target);
                    }
                }

                Items.Clear();
                Items.AddRange(Source.Select(_setup));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                List<object> oldItems = null;
                if (e.OldItems != null)
                {
                    oldItems = new List<object>();
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        var target = Items[e.OldStartingIndex];
                        oldItems.Add(target);
                        _teardown?.Invoke(target);
                        Items.RemoveAt(e.OldStartingIndex);
                    }
                }

                List<object> newItems = null;
                if (e.NewItems != null)
                {
                    newItems = new List<object>();
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        var target = _setup((TSource)e.NewItems[i]);
                        newItems.Add(target);
                        Items.Insert(i + e.NewStartingIndex, target);
                    }
                }

                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove,
                            oldItems,
                            e.OldStartingIndex));
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add,
                            newItems,
                            e.NewStartingIndex));
                }
                else if (e.Action == NotifyCollectionChangedAction.Move)
                {
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Move,
                            newItems,
                            e.NewStartingIndex,
                            e.OldStartingIndex));
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    if (newItems == null)
                    {
                        return;
                    }
                    if (oldItems != null)
                    {
                        OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace,
                                newItems,
                                oldItems,
                                e.NewStartingIndex));
                    }
                }
            }
        }

        #endregion


        #region Fields

        private readonly Func<TSource, TTarget> _setup;

        private readonly Action<TTarget> _teardown;

        private bool _disposed;

        #endregion

        #region Properties

        public IEnumerable<TSource> Source { get; }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Cleanup managed resources
                var notifyCollectionChanged = Source as INotifyCollectionChanged;
                if (notifyCollectionChanged != null)
                {
                    notifyCollectionChanged.CollectionChanged -= OnSourceCollectionChanged;
                }
            }

            // Cleanup unmanaged resources

            // Mark the object as disposed
            _disposed = true;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        #endregion
    }
}