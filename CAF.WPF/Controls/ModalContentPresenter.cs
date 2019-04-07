#region Usings

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

#endregion

namespace CompositeApplicationFramework.Controls
{
    #region Dependencies

    

    #endregion

    [ContentProperty("Content")]
    public class ModalContentPresenter : FrameworkElement
    {
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register(
            "IsModal",
            typeof (bool),
            typeof (ModalContentPresenter),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsModalChanged));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof (object),
            typeof (ModalContentPresenter),
            new UIPropertyMetadata(null, OnContentChanged));

        public static readonly DependencyProperty ModalContentProperty = DependencyProperty.Register(
            "ModalContent",
            typeof (object),
            typeof (ModalContentPresenter),
            new UIPropertyMetadata(null, OnModalContentChanged));

        public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register(
            "OverlayBrush",
            typeof (Brush),
            typeof (ModalContentPresenter),
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(204, 169, 169, 169)), OnOverlayBrushChanged));

        public static readonly RoutedEvent PreviewModalContentShownEvent =
            EventManager.RegisterRoutedEvent(
                "PreviewModalContentShown",
                RoutingStrategy.Tunnel,
                typeof (RoutedEventArgs),
                typeof (ModalContentPresenter));

        public static readonly RoutedEvent ModalContentShownEvent = EventManager.RegisterRoutedEvent(
            "ModalContentShown",
            RoutingStrategy.Bubble,
            typeof (RoutedEventArgs),
            typeof (ModalContentPresenter));

        public static readonly RoutedEvent PreviewModalContentHiddenEvent =
            EventManager.RegisterRoutedEvent(
                "PreviewModalContentHidden",
                RoutingStrategy.Tunnel,
                typeof (RoutedEventArgs),
                typeof (ModalContentPresenter));

        public static readonly RoutedEvent ModalContentHiddenEvent =
            EventManager.RegisterRoutedEvent(
                "ModalContentHidden",
                RoutingStrategy.Bubble,
                typeof (RoutedEventArgs),
                typeof (ModalContentPresenter));

        private static readonly TraversalRequest TraversalDirection;
        private readonly Panel _layoutRoot;
        private readonly object[] _logicalChildren;
        private readonly ContentPresenter _modalContent;
        private readonly Border _overlay;
        private readonly ContentPresenter _primaryContent;
        private KeyboardNavigationMode _cachedKeyboardNavigationMode;

        static ModalContentPresenter()
        {
            TraversalDirection = new TraversalRequest(FocusNavigationDirection.First);
        }

        public ModalContentPresenter()
        {
            _layoutRoot = new Grid();
            _layoutRoot.Background = null;
            _primaryContent = new ContentPresenter();
            _modalContent = new ContentPresenter();
            _overlay = new Border();

            AddVisualChild(_layoutRoot);

            _logicalChildren = new object[2];

            _overlay.Background = OverlayBrush;
            _overlay.Child = _modalContent;
            _overlay.Visibility = Visibility.Hidden;

            _layoutRoot.Children.Add(_primaryContent);
            _layoutRoot.Children.Add(_overlay);
        }

        public bool IsModal
        {
            get => (bool) GetValue(IsModalProperty);
            set => SetValue(IsModalProperty, value);
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public object ModalContent
        {
            get => GetValue(ModalContentProperty);
            set => SetValue(ModalContentProperty, value);
        }

        public Brush OverlayBrush
        {
            get => (Brush) GetValue(OverlayBrushProperty);
            set => SetValue(OverlayBrushProperty, value);
        }

        protected override int VisualChildrenCount => 1;

        protected override IEnumerator LogicalChildren => _logicalChildren.GetEnumerator();

        public event RoutedEventHandler PreviewModalContentShown
        {
            add => AddHandler(PreviewModalContentShownEvent, value);
            remove => RemoveHandler(PreviewModalContentShownEvent, value);
        }

        public event RoutedEventHandler ModalContentShown
        {
            add => AddHandler(ModalContentShownEvent, value);
            remove => RemoveHandler(ModalContentShownEvent, value);
        }

        public event RoutedEventHandler PreviewModalContentHidden
        {
            add => AddHandler(PreviewModalContentHiddenEvent, value);
            remove => RemoveHandler(PreviewModalContentHiddenEvent, value);
        }

        public event RoutedEventHandler ModalContentHidden
        {
            add => AddHandler(ModalContentHiddenEvent, value);
            remove => RemoveHandler(ModalContentHiddenEvent, value);
        }

        public void ShowModalContent()
        {
            if (!IsModal)
            {
                IsModal = true;
            }
        }

        public void HideModalContent()
        {
            if (IsModal)
            {
                IsModal = false;
            }
        }

        protected virtual void OnPreviewModalContentShown(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected virtual void OnModalContentShown(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected virtual void OnPreviewModalContentHidden(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected virtual void OnModalContentHidden(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _layoutRoot;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _layoutRoot.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _layoutRoot.Measure(availableSize);
            return _layoutRoot.DesiredSize;
        }

        private static void OnIsModalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ModalContentPresenter) d;

            if ((bool) e.NewValue)
            {
                control._cachedKeyboardNavigationMode = KeyboardNavigation.GetTabNavigation(control._primaryContent);
                KeyboardNavigation.SetTabNavigation(control._primaryContent, KeyboardNavigationMode.None);

                control._overlay.Visibility = Visibility.Visible;
                control._overlay.MoveFocus(TraversalDirection);

                control.RaiseModalContentShownEvents();
            }
            else
            {
                control._overlay.Visibility = Visibility.Hidden;

                KeyboardNavigation.SetTabNavigation(control._primaryContent, control._cachedKeyboardNavigationMode);
                control._primaryContent.MoveFocus(TraversalDirection);

                control.RaiseModalContentHiddenEvents();
            }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ModalContentPresenter) d;

            if (e.OldValue != null)
            {
                control.RemoveLogicalChild(e.OldValue);
            }

            control._primaryContent.Content = e.NewValue;
            control.AddLogicalChild(e.NewValue);
            control._logicalChildren[0] = e.NewValue;
            control.UpdateLayout();
        }

        private static void OnModalContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ModalContentPresenter) d;

            if (e.OldValue != null)
            {
                control.RemoveLogicalChild(e.OldValue);
            }

            control._modalContent.Content = e.NewValue;
            control.AddLogicalChild(e.NewValue);
            control._logicalChildren[1] = e.NewValue;
            control.UpdateLayout();
        }

        private static void OnOverlayBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ModalContentPresenter) d;
            control._overlay.Background = (Brush) e.NewValue;
        }

        private void RaiseModalContentShownEvents()
        {
            var args = new RoutedEventArgs(PreviewModalContentShownEvent);
            OnPreviewModalContentShown(args);
            if (!args.Handled)
            {
                args = new RoutedEventArgs(ModalContentShownEvent);
                OnModalContentShown(args);
            }
        }

        private void RaiseModalContentHiddenEvents()
        {
            var args = new RoutedEventArgs(PreviewModalContentHiddenEvent);
            OnPreviewModalContentHidden(args);
            if (!args.Handled)
            {
                args = new RoutedEventArgs(ModalContentHiddenEvent);
                OnModalContentHidden(args);
            }
        }
    }
}