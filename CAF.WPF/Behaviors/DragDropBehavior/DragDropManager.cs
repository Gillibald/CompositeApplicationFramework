#region Usings

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CompositeApplicationFramework.Behaviors.DragDropBehavior.DragDropAdorner;
using CompositeApplicationFramework.Extensions;
using CompositeApplicationFramework.Helper;
using CompositeApplicationFramework.Icons;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    public static class DragDropManager
    {
        public static readonly DependencyProperty DragDropProviderProperty =
            DependencyProperty.RegisterAttached(
                "DragDropProvider",
                typeof(IDragDropProvider),
                typeof(DragDropManager));

        public static readonly DependencyProperty EffectNoneAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectNoneAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropManager));

        public static readonly DependencyProperty EffectCopyAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectCopyAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropManager));

        public static readonly DependencyProperty EffectMoveAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectMoveAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropManager));

        public static readonly DependencyProperty EffectLinkAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectLinkAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropManager));

        public static readonly DependencyProperty EffectAllAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectAllAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropManager));

        public static readonly DependencyProperty EffectScrollAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectScrollAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropManager));

        public static readonly DependencyProperty DragAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate),
                typeof(DragDropManager));

        public static readonly DependencyProperty UseDefaultDragAdornerProperty =
            DependencyProperty.RegisterAttached(
                "UseDefaultDragAdorner",
                typeof(bool),
                typeof(DragDropManager),
                new PropertyMetadata(false));

        public static readonly DependencyProperty UseDefaultEffectDataTemplateProperty =
            DependencyProperty.RegisterAttached(
                "UseDefaultEffectDataTemplate",
                typeof(bool),
                typeof(DragDropManager),
                new PropertyMetadata(false));

        public static readonly DependencyProperty DragHandlerProperty =
            DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(DragDropManager));

        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDropManager));

        public static readonly DependencyProperty DragSourceIgnoreProperty =
            DependencyProperty.RegisterAttached(
                "DragSourceIgnore",
                typeof(bool),
                typeof(DragDropManager),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached(
                "IsDragSource",
                typeof(bool),
                typeof(DragDropManager),
                new UIPropertyMetadata(false, IsDragSourceChanged));

        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached(
                "IsDropTarget",
                typeof(bool),
                typeof(DragDropManager),
                new UIPropertyMetadata(false, IsDropTargetChanged));

        public static readonly DataFormat DataFormat = DataFormats.GetDataFormat("CAF");
        private static IDragSource _defaultDragHandler;
        private static IDropTarget _defaultDropHandler;
        private static IDragDropProvider _defaultDragDropProvider;
        private static DragAdorner _dragAdorner;
        private static DragAdorner _effectAdorner;
        private static IDragInfo _dragInfo;
        private static bool _dragInProgress;
        private static DropTargetAdorner _dropTargetAdorner;
        private static object _clickSupressItem;
        private static Point _adornerPos;
        private static Size _adornerSize;

        public static IDragSource DefaultDragHandler
        {
            get => _defaultDragHandler ?? (_defaultDragHandler = new DefaultDragHandler());
            set => _defaultDragHandler = value;
        }

        public static IDropTarget DefaultDropHandler
        {
            get => _defaultDropHandler ?? (_defaultDropHandler = new DefaultDropHandler());
            set => _defaultDropHandler = value;
        }

        public static IDragDropProvider DefaultDropDropProvider
        {
            get => _defaultDragDropProvider ?? (_defaultDragDropProvider = new DefaultDragDropProvider());
            set => _defaultDragDropProvider = value;
        }

        private static DragAdorner DragAdorner
        {
            get => _dragAdorner;
            set
            {
                _dragAdorner?.Destroy();

                _dragAdorner = value;
            }
        }

        private static DragAdorner EffectAdorner
        {
            get => _effectAdorner;
            set
            {
                _effectAdorner?.Destroy();

                _effectAdorner = value;
            }
        }

        private static DropTargetAdorner DropTargetAdorner
        {
            get => _dropTargetAdorner;
            set
            {
                _dropTargetAdorner?.Detatch();

                _dropTargetAdorner = value;
            }
        }

        public static IDragDropProvider GetDragDropProvider(UIElement pTarget)
        {
            return (IDragDropProvider)pTarget.GetValue(DragDropProviderProperty);
        }

        public static void SetDragDropProvider(UIElement pTarget, IDragDropProvider pValue)
        {
            pTarget.SetValue(DragDropProviderProperty, pValue);
        }

        public static DataTemplate GetDragAdornerTemplate(UIElement pTarget)
        {
            return (DataTemplate)pTarget.GetValue(DragAdornerTemplateProperty);
        }

        public static void SetDragAdornerTemplate(UIElement pTarget, DataTemplate pValue)
        {
            pTarget.SetValue(DragAdornerTemplateProperty, pValue);
        }

        public static bool GetUseDefaultDragAdorner(UIElement pTarget)
        {
            return (bool)pTarget.GetValue(UseDefaultDragAdornerProperty);
        }

        public static void SetUseDefaultDragAdorner(UIElement pTarget, bool pValue)
        {
            pTarget.SetValue(UseDefaultDragAdornerProperty, pValue);
        }

        public static bool GetUseDefaultEffectDataTemplate(UIElement pTarget)
        {
            return (bool)pTarget.GetValue(UseDefaultEffectDataTemplateProperty);
        }

        public static void SetUseDefaultEffectDataTemplate(UIElement pTarget, bool pValue)
        {
            pTarget.SetValue(UseDefaultEffectDataTemplateProperty, pValue);
        }

        public static DataTemplate GetEffectNoneAdornerTemplate(UIElement pTarget)
        {
            var lTemplate = pTarget.GetValue(EffectNoneAdornerTemplateProperty) as DataTemplate;

            if (lTemplate != null) return lTemplate;
            if (!GetUseDefaultEffectDataTemplate(pTarget))
            {
                return null;
            }

            var lImageSourceFactory = new FrameworkElementFactory(typeof(Image));
            lImageSourceFactory.SetValue(Image.SourceProperty, IconFactory.EffectNone);
            lImageSourceFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
            lImageSourceFactory.SetValue(FrameworkElement.WidthProperty, 12.0);

            lTemplate = new DataTemplate { VisualTree = lImageSourceFactory };

            return lTemplate;
        }

        public static void SetEffectNoneAdornerTemplate(UIElement pTarget, DataTemplate pValue)
        {
            pTarget.SetValue(EffectNoneAdornerTemplateProperty, pValue);
        }

        public static DataTemplate GetEffectCopyAdornerTemplate(UIElement pTarget, string pDestinationText)
        {
            var template = (DataTemplate)pTarget.GetValue(EffectCopyAdornerTemplateProperty) ??
                           CreateDefaultEffectDataTemplate(pTarget, IconFactory.EffectCopy, "Copy to", pDestinationText);

            return template;
        }

        public static void SetEffectCopyAdornerTemplate(UIElement pTarget, DataTemplate pValue)
        {
            pTarget.SetValue(EffectCopyAdornerTemplateProperty, pValue);
        }

        public static DataTemplate GetEffectMoveAdornerTemplate(UIElement pTarget, string pDestinationText)
        {
            var lTemplate = pTarget.GetValue(EffectMoveAdornerTemplateProperty) as DataTemplate ??
                            CreateDefaultEffectDataTemplate(
                                pTarget,
                                IconFactory.EffectMove,
                                "Move to",
                                pDestinationText);

            return lTemplate;
        }

        public static void SetEffectMoveAdornerTemplate(UIElement pTarget, DataTemplate pValue)
        {
            pTarget.SetValue(EffectMoveAdornerTemplateProperty, pValue);
        }

        public static DataTemplate GetEffectLinkAdornerTemplate(UIElement pTarget, string pDestinationText)
        {
            var pTemplate = pTarget.GetValue(EffectLinkAdornerTemplateProperty) as DataTemplate ??
                            CreateDefaultEffectDataTemplate(
                                pTarget,
                                IconFactory.EffectLink,
                                "Link to",
                                pDestinationText);

            return pTemplate;
        }

        public static void SetEffectLinkAdornerTemplate(UIElement pTarget, DataTemplate pValue)
        {
            pTarget.SetValue(EffectLinkAdornerTemplateProperty, pValue);
        }

        public static DataTemplate GetEffectAllAdornerTemplate(UIElement pTarget)
        {
            var lTemplate = pTarget.GetValue(EffectAllAdornerTemplateProperty) as DataTemplate;

            // TODO: Add default template

            return lTemplate;
        }

        public static void SetEffectAllAdornerTemplate(UIElement pTarget, DataTemplate pValue)
        {
            pTarget.SetValue(EffectAllAdornerTemplateProperty, pValue);
        }

        public static DataTemplate GetEffectScrollAdornerTemplate(UIElement pTarget)
        {
            var lTemplate = pTarget.GetValue(EffectScrollAdornerTemplateProperty) as DataTemplate;

            // TODO: Add default template

            return lTemplate;
        }

        public static void SetEffectScrollAdornerTemplate(UIElement pTarget, DataTemplate pValue)
        {
            pTarget.SetValue(EffectScrollAdornerTemplateProperty, pValue);
        }

        public static bool GetIsDragSource(UIElement pTarget)
        {
            return (bool)pTarget.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(UIElement pTarget, bool pValue)
        {
            pTarget.SetValue(IsDragSourceProperty, pValue);
        }

        public static bool GetIsDropTarget(UIElement pTarget)
        {
            return (bool)pTarget.GetValue(IsDropTargetProperty);
        }

        public static void SetIsDropTarget(UIElement pTarget, bool pValue)
        {
            pTarget.SetValue(IsDropTargetProperty, pValue);
        }

        public static IDragSource GetDragHandler(UIElement pTarget)
        {
            return (IDragSource)pTarget.GetValue(DragHandlerProperty);
        }

        public static void SetDragHandler(UIElement pTarget, IDragSource pValue)
        {
            pTarget.SetValue(DragHandlerProperty, pValue);
        }

        public static IDropTarget GetDropHandler(UIElement pTarget)
        {
            return (IDropTarget)pTarget.GetValue(DropHandlerProperty);
        }

        public static void SetDropHandler(UIElement pTarget, IDropTarget pValue)
        {
            pTarget.SetValue(DropHandlerProperty, pValue);
        }

        public static bool GetDragSourceIgnore(UIElement pTarget)
        {
            return (bool)pTarget.GetValue(DragSourceIgnoreProperty);
        }

        public static void SetDragSourceIgnore(UIElement pTarget, bool pValue)
        {
            pTarget.SetValue(DragSourceIgnoreProperty, pValue);
        }

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lUiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                lUiElement.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
                lUiElement.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
                lUiElement.PreviewMouseMove += DragSource_PreviewMouseMove;
                lUiElement.QueryContinueDrag += DragSource_QueryContinueDrag;
            }
            else
            {
                lUiElement.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                lUiElement.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
                lUiElement.PreviewMouseMove -= DragSource_PreviewMouseMove;
                lUiElement.QueryContinueDrag -= DragSource_QueryContinueDrag;
            }
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)d;

            if ((bool)e.NewValue)
            {
                element.AllowDrop = true;

                element.DragEnter += DropTarget_PreviewDragEnter;
                element.DragLeave += DropTarget_PreviewDragLeave;
                element.DragOver += DropTarget_PreviewDragOver;
                element.Drop += DropTarget_PreviewDrop;
                element.GiveFeedback += DropTarget_GiveFeedback;
            }
            else
            {
                element.AllowDrop = false;

                element.DragEnter -= DropTarget_PreviewDragEnter;
                element.DragLeave -= DropTarget_PreviewDragLeave;
                element.DragOver -= DropTarget_PreviewDragOver;
                element.Drop -= DropTarget_PreviewDrop;
                element.GiveFeedback -= DropTarget_GiveFeedback;

                Mouse.OverrideCursor = null;
            }
        }

        private static void CreateDragAdorner()
        {
            UIElement lRootElement = null;
            var pParentWindow = _dragInfo.VisualSource.GetVisualAncestor<Window>();

            if (pParentWindow != null)
            {
                lRootElement = pParentWindow.Content as UIElement;
            }
            if (lRootElement == null)
            {
                if (Application.Current.MainWindow != null)
                    lRootElement = (UIElement)Application.Current.MainWindow.Content;
            }

            if (lRootElement == null) return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(lRootElement);

            var adornerTemplate = GetUseDefaultDragAdorner(_dragInfo.VisualSource)
                ? GetDefaultDragAdornerTemplate()
                : GetDragAdornerTemplate(_dragInfo.VisualSource);

            DragAdorner = new DragAdorner(_dragInfo.Data, adornerTemplate, lRootElement, adornerLayer);
        }

        private static DataTemplate GetDefaultDragAdornerTemplate()
        {
            var dataTemplate = new DataTemplate();

            var lSize = _dragInfo.VisualSourceItem is TreeViewItem
                ? _dragInfo.VisualSourceItem.DesiredSize
                : _dragInfo.VisualSourceItem.RenderSize;

            var lVisualBrush = new VisualBrush(_dragInfo.VisualSourceItem);

            var rectangleFactory = new FrameworkElementFactory(typeof(Border));
            rectangleFactory.SetValue(Border.BackgroundProperty, lVisualBrush.CloneCurrentValue());
            rectangleFactory.SetValue(FrameworkElement.WidthProperty, lSize.Width);
            rectangleFactory.SetValue(FrameworkElement.HeightProperty, lSize.Height);
            rectangleFactory.SetValue(UIElement.IsHitTestVisibleProperty, false);
            rectangleFactory.SetValue(UIElement.OpacityProperty, 0.5);

            //set the visual tree of the data template
            dataTemplate.VisualTree = rectangleFactory;

            return dataTemplate;
        }

        private static void CreateEffectAdorner(IDropInfo pDropInfo)
        {
            var lTemplate = GetEffectAdornerTemplate(
                _dragInfo.VisualSource,
                pDropInfo.Effects,
                pDropInfo.DestinationText);

            if (lTemplate == null) return;

            if (Application.Current.MainWindow == null) return;

            var lRootElement = (UIElement)Application.Current.MainWindow.Content;

            var adornerLayer = AdornerLayer.GetAdornerLayer(lRootElement);

            EffectAdorner = new DragAdorner(_dragInfo.Data, lTemplate, lRootElement, adornerLayer);
        }

        private static DataTemplate CreateDefaultEffectDataTemplate(
            UIElement pTarget,
            BitmapImage pEffectIcon,
            string pEffectText,
            string pDestinationText)
        {
            if (!GetUseDefaultEffectDataTemplate(pTarget))
            {
                return null;
            }

            // Add icon
            var lImageFactory = new FrameworkElementFactory(typeof(Image));
            lImageFactory.SetValue(Image.SourceProperty, pEffectIcon);
            lImageFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
            lImageFactory.SetValue(FrameworkElement.WidthProperty, 12.0);
            lImageFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0.0, 0.0, 3.0, 0.0));

            // Add effect text
            var lEffectTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            lEffectTextBlockFactory.SetValue(TextBlock.TextProperty, pEffectText);
            lEffectTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            lEffectTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.Blue);

            // Add destination text
            var lDestinationTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            lDestinationTextBlockFactory.SetValue(TextBlock.TextProperty, pDestinationText);
            lDestinationTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            lDestinationTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.DarkBlue);
            lDestinationTextBlockFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(3, 0, 0, 0));
            lDestinationTextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.DemiBold);

            // Create containing panel
            var lStackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            lStackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            lStackPanelFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(2.0));
            lStackPanelFactory.AppendChild(lImageFactory);
            lStackPanelFactory.AppendChild(lEffectTextBlockFactory);
            lStackPanelFactory.AppendChild(lDestinationTextBlockFactory);

            // Add border
            var lBorderFactory = new FrameworkElementFactory(typeof(Border));
            var lStopCollection = new GradientStopCollection
            {
                new GradientStop(Colors.White, 0.0),
                new GradientStop(Colors.AliceBlue, 1.0)
            };

            var lGradientBrush = new LinearGradientBrush(lStopCollection)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            lBorderFactory.SetValue(Panel.BackgroundProperty, lGradientBrush);
            lBorderFactory.SetValue(Border.BorderBrushProperty, Brushes.DimGray);
            lBorderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3.0));
            lBorderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
            lBorderFactory.AppendChild(lStackPanelFactory);

            // Finally add content to template
            var lTemplate = new DataTemplate { VisualTree = lBorderFactory };
            return lTemplate;
        }

        private static DataTemplate GetEffectAdornerTemplate(
            UIElement pTarget,
            DragDropEffects pEffect,
            string pDestinationText)
        {
            switch (pEffect)
            {
                case DragDropEffects.All:
                    {
                        return null;
                    }
                case DragDropEffects.Copy:
                    {
                        return GetEffectCopyAdornerTemplate(pTarget, pDestinationText);
                    }
                case DragDropEffects.Link:
                    {
                        return GetEffectLinkAdornerTemplate(pTarget, pDestinationText);
                    }
                case DragDropEffects.Move:
                    {
                        return GetEffectMoveAdornerTemplate(pTarget, pDestinationText);
                    }
                case DragDropEffects.None:
                    {
                        return GetEffectNoneAdornerTemplate(pTarget);
                    }
                case DragDropEffects.Scroll:
                    {
                        return null;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        private static bool HitTestForType<T>(object sender, MouseEventArgs e) where T : UIElement
        {
            var lHit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
            if (lHit == null)
            {
                return false;
            }
            UIElement lScrollBar = lHit.VisualHit.GetVisualAncestor<T>();
            return lScrollBar != null && lScrollBar.Visibility == Visibility.Visible;
        }

        private static void Scroll(DependencyObject o, DragEventArgs e)
        {
            var lScrollViewer = o.GetVisualDescendent<ScrollViewer>();

            if (lScrollViewer != null)
            {
                var pPosition = e.GetPosition(lScrollViewer);
                var lScrollMargin = Math.Min(lScrollViewer.FontSize * 2, lScrollViewer.ActualHeight / 2);

                if (pPosition.X >= lScrollViewer.ActualWidth - lScrollMargin
                    && lScrollViewer.HorizontalOffset < lScrollViewer.ExtentWidth - lScrollViewer.ViewportWidth)
                {
                    lScrollViewer.LineRight();
                }
                else
                {
                    if (pPosition.X < lScrollMargin && lScrollViewer.HorizontalOffset > 0)
                    {
                        lScrollViewer.LineLeft();
                    }
                    else
                    {
                        if (pPosition.Y >= lScrollViewer.ActualHeight - lScrollMargin
                            && lScrollViewer.VerticalOffset < lScrollViewer.ExtentHeight - lScrollViewer.ViewportHeight)
                        {
                            lScrollViewer.LineDown();
                        }
                        else
                        {
                            if (pPosition.Y < lScrollMargin && lScrollViewer.VerticalOffset > 0)
                            {
                                lScrollViewer.LineUp();
                            }
                        }
                    }
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
            if (e.ClickCount != 1 || HitTestForType<ScrollBar>(sender, e) || HitTestForType<TextBoxBase>(sender, e)
                || HitTestForType<PasswordBox>(sender, e) || HitTestForType<Slider>(sender, e)
                || GetDragSourceIgnore((UIElement)sender))
            {
                _dragInfo = null;
                return;
            }

            var dragDropProvider = GetDragDropProvider((UIElement)sender) ?? DefaultDropDropProvider;

            _dragInfo = dragDropProvider.GetDragInfo(sender, e);

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.
            var lItemsControl = sender as ItemsControl;

            if (_dragInfo.VisualSourceItem == null || lItemsControl == null || !lItemsControl.CanSelectMultipleItems())
            {
                return;
            }

            var lSelectedItems = lItemsControl.GetSelectedItems().Cast<object>();

            var selectedItems = lSelectedItems as object[] ?? lSelectedItems.ToArray();

            if (selectedItems.Length <= 1 || !selectedItems.Contains(_dragInfo.SourceItem))
            {
                return;
            }
            _clickSupressItem = _dragInfo.SourceItem;

            e.Handled = true;
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
            // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
            var lItemsControl = sender as ItemsControl;

            if (lItemsControl != null && _dragInfo != null && _clickSupressItem == _dragInfo.SourceItem)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    lItemsControl.SetItemSelected(_dragInfo.SourceItem, false);
                }
                else
                {
                    lItemsControl.SetSelectedItem(_dragInfo.SourceItem);
                }
            }

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_dragInfo != null)
            {
                _dragInfo = null;
            }

            _clickSupressItem = null;
        }

        private static IDragSource TryGetDragHandler(IDragInfo pDragInfo, UIElement pSender)
        {
            IDragSource lDragHandler = null;
            if (pDragInfo?.VisualSource != null)
            {
                lDragHandler = GetDragHandler(_dragInfo.VisualSource);
            }
            if (lDragHandler == null && pSender != null)
            {
                lDragHandler = GetDragHandler(pSender);
            }
            return lDragHandler ?? DefaultDragHandler;
        }

        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragInfo == null || _dragInProgress) return;
            var lDragStart = _dragInfo.DragStartPosition;
            var lPosition = e.GetPosition((IInputElement)sender);

            if (!(Math.Abs(lPosition.X - lDragStart.X) > SystemParameters.MinimumHorizontalDragDistance) &&
                !(Math.Abs(lPosition.Y - lDragStart.Y) > SystemParameters.MinimumVerticalDragDistance)) return;
            var lDragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
            lDragHandler.StartDrag(_dragInfo);

            if (_dragInfo.Effects == DragDropEffects.None || _dragInfo.Data == null) return;
            var lData = _dragInfo.DataObject;

            if (lData == null)
            {
                lData = new DataObject(DataFormat.Name, _dragInfo.Data);
            }
            else
            {
                lData.SetData(DataFormat.Name, _dragInfo.Data);
            }

            try
            {
                _dragInProgress = true;
                if (DragDrop.DoDragDrop(_dragInfo.VisualSource, lData, _dragInfo.Effects) == DragDropEffects.None)
                {
                    lDragHandler.CancelDrag(_dragInfo);
                }
            }
            finally
            {
                _dragInProgress = false;
            }

            _dragInfo = null;
        }

        private static void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed) return;

            e.Action = DragAction.Cancel;

            CleanUpAdorner();
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTarget_PreviewDragOver(sender, e);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            CleanUpAdorner();

            Mouse.OverrideCursor = null;
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var lDropInfo = new DropInfo(sender, e, _dragInfo);
            var lDropHandler = GetDropHandler((UIElement)sender);
            var lItemsControl = sender as ItemsControl;

            if (lDropHandler != null)
            {
                lDropHandler.DragOver(lDropInfo);
            }
            else
            {
                DefaultDropHandler.DragOver(lDropInfo);
            }

            if (DragAdorner == null && _dragInfo != null)
            {
                CreateDragAdorner();
            }

            if (DragAdorner != null)
            {
                var lTempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

                if (lTempAdornerPos.X > 0 && lTempAdornerPos.Y > 0)
                {
                    _adornerPos = lTempAdornerPos;
                }

                // Fixed the flickering adorner - Size changes to zero 'randomly'...?
                if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0)
                {
                    _adornerSize = DragAdorner.RenderSize;
                }

                if (_dragInfo != null)
                {
                    // When there is a custom adorner move to above the cursor and center it
                    if (GetDragAdornerTemplate(_dragInfo.VisualSource) != null)
                    {
                        _adornerPos.Offset((_adornerSize.Width * -0.5), (_adornerSize.Height * -0.5));
                    }
                    else
                    {
                        _adornerPos.Offset(
                            _dragInfo.PositionInDraggedItem.X * -1,
                            _dragInfo.PositionInDraggedItem.Y * -1);
                    }
                }
                DragAdorner.UpdatePosition(_adornerPos);
            }

            //If the target is an ItemsControl then update the drop target adorner.
            if (lItemsControl != null)
            {
                // Display the adorner in the control's ItemsPresenter. If there is no 
                // ItemsPresenter provided by the style, try getting hold of a
                // ScrollContentPresenter and using that.
                var lAdornedElement = lItemsControl.GetVisualDescendent<ItemsPresenter>()
                                      ?? (UIElement)lItemsControl.GetVisualDescendent<ScrollContentPresenter>();

                if (lAdornedElement != null)
                {
                    if (lDropInfo.DropTargetAdorner == null)
                    {
                        DropTargetAdorner = null;
                    }
                    else if (!lDropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                    {
                        DropTargetAdorner = DropTargetAdorner.Create(lDropInfo.DropTargetAdorner, lAdornedElement);
                    }

                    if (DropTargetAdorner != null)
                    {
                        DropTargetAdorner.DropInfo = lDropInfo;
                        DropTargetAdorner.InvalidateVisual();
                    }
                }
            }

            // Set the drag effect adorner if there is one
            if (EffectAdorner == null && _dragInfo != null)
            {
                CreateEffectAdorner(lDropInfo);
            }

            if (EffectAdorner != null)
            {
                var lAdornerPos = e.GetPosition(EffectAdorner.AdornedElement);

                lAdornerPos.Offset(20, 20);

                EffectAdorner.UpdatePosition(lAdornerPos);
            }

            e.Effects = lDropInfo.Effects;

            e.Handled = true;

            Scroll((DependencyObject)sender, e);
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            var lDropInfo = new DropInfo(sender, e, _dragInfo);

            var lDropHandler = GetDropHandler((UIElement)sender) ?? DefaultDropHandler;

            var lDragHandler = TryGetDragHandler(_dragInfo, (UIElement)sender);

            CleanUpAdorner();

            lDropHandler.Drop(lDropInfo);

            lDragHandler.Dropped(lDropInfo);

            Mouse.OverrideCursor = null;

            e.Handled = true;
        }

        private static void CleanUpAdorner()
        {
            DragAdorner?.Destroy();
            DragAdorner = null;

            EffectAdorner?.Destroy();
            EffectAdorner = null;

            DropTargetAdorner?.Detatch();
            DropTargetAdorner = null;
        }

        private static void DropTarget_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (EffectAdorner != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
            }
            else
            {                
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }
    }
}