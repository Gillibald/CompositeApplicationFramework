using CompositeApplicationFramework.Commands;

namespace CompositeApplicationFramework.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using JetBrains.Annotations;

    public class ListPager : Freezable
    {
        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register(
            "PageSize",
            typeof(int),
            typeof(ListPager));

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(
            "CurrentPage",
            typeof(int),
            typeof(ListPager),
            new FrameworkPropertyMetadata(1));

        public static readonly DependencyProperty PageCountProperty = DependencyProperty.Register("PageCount", typeof(int), typeof(ListPager));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(IEnumerable), typeof(ListPager), new FrameworkPropertyMetadata(SourceChanged));

        private static readonly DependencyPropertyKey ItemsPropertyKey = DependencyProperty.RegisterReadOnly(
            "Items",
            typeof(IList),
            typeof(ListPager), new FrameworkPropertyMetadata(new ObservableCollection<object>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        private DelegateCommand _previousCommand;
        private DelegateCommand _nextCommand;
        private DelegateCommand _firstCommand;
        private DelegateCommand _lastCommand;

        public DelegateCommand PreviousCommand => _previousCommand ?? (_previousCommand = new DelegateCommand(MoveToPreviousPage, CanMoveToPreviousPage));
        public DelegateCommand NextCommand => _nextCommand ?? (_nextCommand = new DelegateCommand(MoveToNextPage, CanMoveToNextPage));
        public DelegateCommand FirstCommand => _firstCommand ?? (_firstCommand = new DelegateCommand(MoveToFirstPage, CanMoveToFirstPage));
        public DelegateCommand LastCommand => _lastCommand ?? (_lastCommand = new DelegateCommand(MoveToLastPage, CanMoveToLastPage));

        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public int PageCount
        {
            get => (int)GetValue(PageCountProperty);
            set => SetValue(PageCountProperty, value);
        }

        public IList Items
        {
            get => (IList)GetValue(ItemsProperty);
            protected set => SetValue(ItemsPropertyKey, value);
        }

        public IEnumerable Source
        {
            get => (IEnumerable)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private ICollectionView View { get; set; }

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listPager = (ListPager)d;

            listPager.OnSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void OnSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var notifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged -= SourceCollectionChanged;
            }

            if (newValue == null)
            {
                SourceCollection = null;
                Items = null;
                View = null;
                return;
            }

            var collectionView = Source as ICollectionView;

            if (collectionView != null)
            {
                SourceCollection = new EnumerableWrapper(collectionView);
            }
            else
            {
                SourceCollection = newValue as IList;
            }

            if (SourceCollection == null) return;

            View = CollectionViewSource.GetDefaultView(SourceCollection);

            Items = new EnumerableWrapper(View);

            Refresh();

            notifyCollectionChanged = newValue as INotifyCollectionChanged;

            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += SourceCollectionChanged;
            }
        }


        private IList SourceCollection { get; set; }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        private bool CanMoveToNextPage()
        {
            if (Source == null || Items.Count == 0)
            {
                return false;
            }
            return CurrentPage != PageCount;
        }

        private void MoveToNextPage()
        {
            CurrentPage++;
            Refresh();
        }

        private bool CanMoveToPreviousPage()
        {
            if (Source == null)
            {
                return false;
            }
            return CurrentPage != 1;
        }

        private void MoveToPreviousPage()
        {
            CurrentPage--;
            Refresh();
        }

        private bool CanMoveToFirstPage()
        {
            if (Source == null)
            {
                return false;
            }
            return CurrentPage != 1;
        }

        private void MoveToFirstPage()
        {
            CurrentPage = 1;
            Refresh();
        }

        private bool CanMoveToLastPage()
        {
            if (Source == null)
            {
                return false;
            }
            return CurrentPage != PageCount;
        }

        private void MoveToLastPage()
        {
            CurrentPage = PageCount;
            Refresh();
        }

        private void Refresh()
        {
            CalculatePageCount();
            View.Filter = Filter;
            RefreshCommands();
        }

        private void RefreshCommands()
        {
            PreviousCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
            FirstCommand.RaiseCanExecuteChanged();
            LastCommand.RaiseCanExecuteChanged();
        }

        private bool Filter(object obj)
        {
            var index = SourceCollection.IndexOf(obj);

            var low = PageSize * (CurrentPage - 1);

            var high = PageSize * CurrentPage - 1;

            return low <= index && index <= high;
        }

        private void CalculatePageCount()
        {
            PageCount = (SourceCollection.Count / PageSize);

            if (SourceCollection.Count % PageSize != 0)
            {
                PageCount += 1;
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ListPager();
        }

        private class EnumerableWrapper : List<object>, INotifyCollectionChanged
        {
            private readonly IEnumerable _source;

            public EnumerableWrapper([NotNull] IEnumerable source)
            {
                _source = source;

                foreach (var item in _source)
                {
                    Add(item);
                }

                var notifyCollectionChanged = source as INotifyCollectionChanged;

                if (notifyCollectionChanged != null)
                {
                    notifyCollectionChanged.CollectionChanged += SourceChanged;
                }
            }

            private void SourceChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if (e.NewStartingIndex != -1)
                            {
                                //todo: AddRange
                                Insert(e.NewStartingIndex, e.NewItems[0]);
                            }
                            else
                            {
                                foreach (var newItem in e.NewItems)
                                {
                                    Add(newItem);
                                }
                            }
                            RaiseCollectionChanged(
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems));
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            foreach (var oldItem in e.OldItems)
                            {
                                Remove(oldItem);
                            }
                            RaiseCollectionChanged(
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems));
                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            this[e.NewStartingIndex] = e.NewItems[0];
                            RaiseCollectionChanged(
                                new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Replace,
                                    e.NewItems,
                                    e.OldItems,
                                    e.NewStartingIndex));
                            break;
                        }
                    case NotifyCollectionChangedAction.Move:
                        {
                            RemoveAt(e.OldStartingIndex);
                            Insert(e.NewStartingIndex, e.NewItems[0]);
                            RaiseCollectionChanged(
                                new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Move,
                                    e.NewItems,
                                    e.NewStartingIndex,
                                    e.OldStartingIndex));
                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            Clear();
                            foreach (var item in _source)
                            {
                                Add(item);
                            }
                            RaiseCollectionChanged(
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(this, e);
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
        }
    }
}
