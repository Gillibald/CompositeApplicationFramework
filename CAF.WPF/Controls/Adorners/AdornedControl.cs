﻿#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

#endregion

namespace CompositeApplicationFramework.Controls.Adorners
{
    public class AdornedControl : ContentControl
    {
        public AdornedControl()
        {
            Focusable = false; // By default don't want 'AdornedControl' to be focusable.

            DataContextChanged += AdornedControl_DataContextChanged;

            _closeAdornerTimer.Tick += CloseAdornerTimerTick;
            _closeAdornerTimer.Interval = TimeSpan.FromSeconds(CloseAdornerTimeOut);
        }

        /// <summary>
        ///     Shows or hides the adorner.
        ///     Set to 'true' to show the adorner or 'false' to hide the adorner.
        /// </summary>
        public bool IsAdornerVisible
        {
            get => (bool) GetValue(IsAdornerVisibleProperty);
            set => SetValue(IsAdornerVisibleProperty, value);
        }

        /// <summary>
        ///     Used in XAML to define the UI content of the adorner.
        /// </summary>
        public FrameworkElement AdornerContent
        {
            get => (FrameworkElement) GetValue(AdornerContentProperty);
            set => SetValue(AdornerContentProperty, value);
        }

        /// <summary>
        ///     Specifies the horizontal placement of the adorner relative to the adorned control.
        /// </summary>
        public AdornerPlacement HorizontalAdornerPlacement
        {
            get => (AdornerPlacement) GetValue(HorizontalAdornerPlacementProperty);
            set => SetValue(HorizontalAdornerPlacementProperty, value);
        }

        /// <summary>
        ///     Specifies the vertical placement of the adorner relative to the adorned control.
        /// </summary>
        public AdornerPlacement VerticalAdornerPlacement
        {
            get => (AdornerPlacement) GetValue(VerticalAdornerPlacementProperty);
            set => SetValue(VerticalAdornerPlacementProperty, value);
        }

        /// <summary>
        ///     X offset of the adorner.
        /// </summary>
        public double AdornerOffsetX
        {
            get => (double) GetValue(AdornerOffsetXProperty);
            set => SetValue(AdornerOffsetXProperty, value);
        }

        /// <summary>
        ///     Y offset of the adorner.
        /// </summary>
        public double AdornerOffsetY
        {
            get => (double) GetValue(AdornerOffsetYProperty);
            set => SetValue(AdornerOffsetYProperty, value);
        }

        /// <summary>
        ///     Set to 'true' to make the adorner automatically fade-in and become visible when the mouse is hovered
        ///     over the adorned control.  Also the adorner automatically fades-out when the mouse cursor is moved
        ///     aware from the adorned control (and the adorner).
        /// </summary>
        public bool IsMouseOverShowEnabled
        {
            get => (bool) GetValue(IsMouseOverShowEnabledProperty);
            set => SetValue(IsMouseOverShowEnabledProperty, value);
        }

        /// <summary>
        ///     Specifies the time (in seconds) it takes to fade in the adorner.
        /// </summary>
        public double FadeInTime
        {
            get => (double) GetValue(FadeInTimeProperty);
            set => SetValue(FadeInTimeProperty, value);
        }

        /// <summary>
        ///     Specifies the time (in seconds) it takes to fade out the adorner.
        /// </summary>
        public double FadeOutTime
        {
            get => (double) GetValue(FadeOutTimeProperty);
            set => SetValue(FadeOutTimeProperty, value);
        }

        /// <summary>
        ///     Specifies the time (in seconds) after the mouse cursor moves away from the
        ///     adorned control (or the adorner) when the adorner begins to fade out.
        /// </summary>
        public double CloseAdornerTimeOut
        {
            get => (double) GetValue(CloseAdornerTimeOutProperty);
            set => SetValue(CloseAdornerTimeOutProperty, value);
        }

        /// <summary>
        ///     By default this property is set to null.
        ///     When set to non-null it specifies the part name of a UI element
        ///     in the visual tree of the AdornedControl content that is to be adorned.
        ///     When this property is null it is the AdornerControl content that is adorned,
        ///     however when it is set the visual-tree is searched for a UI element that has the
        ///     specified part name, if the part is found then that UI element is adorned, otherwise
        ///     an exception "Failed to find part ..." is thrown.        ///
        /// </summary>
        public string AdornedTemplatePartName
        {
            get => (string) GetValue(AdornedTemplatePartNameProperty);
            set => SetValue(AdornedTemplatePartNameProperty, value);
        }

        /// <summary>
        ///     Show the adorner.
        /// </summary>
        public void ShowAdorner()
        {
            IsAdornerVisible = true;
        }

        /// <summary>
        ///     Hide the adorner.
        /// </summary>
        public void HideAdorner()
        {
            IsAdornerVisible = false;
        }

        /// <summary>
        ///     Fade the adorner in and make it visible.
        /// </summary>
        public void FadeInAdorner()
        {
            if (_adornerShowState == AdornerShowState.Visible || _adornerShowState == AdornerShowState.FadingIn)
            {
                // Already visible or fading in.
                return;
            }

            ShowAdorner();

            if (_adornerShowState != AdornerShowState.FadingOut)
            {
                _adorner.Opacity = 0.0;
            }

            var doubleAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(FadeInTime)));
            doubleAnimation.Completed += FadeInAnimationCompleted;
            doubleAnimation.Freeze();

            _adorner.BeginAnimation(OpacityProperty, doubleAnimation);

            _adornerShowState = AdornerShowState.FadingIn;
        }

        /// <summary>
        ///     Fade the adorner out and make it visible.
        /// </summary>
        public void FadeOutAdorner()
        {
            if (_adornerShowState == AdornerShowState.FadingOut)
            {
                //
                // Already fading out.
                //
                return;
            }

            if (_adornerShowState == AdornerShowState.Hidden)
            {
                //
                // Adorner has already been hidden.
                //
                return;
            }

            var fadeOutAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(FadeOutTime)));
            fadeOutAnimation.Completed += FadeOutAnimationCompleted;
            fadeOutAnimation.Freeze();

            _adorner.BeginAnimation(OpacityProperty, fadeOutAnimation);

            _adornerShowState = AdornerShowState.FadingOut;
        }

        #region Dependency Properties

        /// <summary>
        ///     Dependency properties.
        /// </summary>
        public static readonly DependencyProperty IsAdornerVisibleProperty =
            DependencyProperty.Register(
                "IsAdornerVisible",
                typeof (bool),
                typeof (AdornedControl),
                new FrameworkPropertyMetadata(IsAdornerVisible_PropertyChanged));

        public static readonly DependencyProperty AdornerContentProperty = DependencyProperty.Register(
            "AdornerContent",
            typeof (FrameworkElement),
            typeof (AdornedControl),
            new FrameworkPropertyMetadata(AdornerContent_PropertyChanged));

        public static readonly DependencyProperty HorizontalAdornerPlacementProperty =
            DependencyProperty.Register(
                "HorizontalAdornerPlacement",
                typeof (AdornerPlacement),
                typeof (AdornedControl),
                new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        public static readonly DependencyProperty VerticalAdornerPlacementProperty =
            DependencyProperty.Register(
                "VerticalAdornerPlacement",
                typeof (AdornerPlacement),
                typeof (AdornedControl),
                new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        public static readonly DependencyProperty AdornerOffsetXProperty = DependencyProperty.Register(
            "AdornerOffsetX",
            typeof (double),
            typeof (AdornedControl));

        public static readonly DependencyProperty AdornerOffsetYProperty = DependencyProperty.Register(
            "AdornerOffsetY",
            typeof (double),
            typeof (AdornedControl));

        public static readonly DependencyProperty IsMouseOverShowEnabledProperty =
            DependencyProperty.Register(
                "IsMouseOverShowEnabled",
                typeof (bool),
                typeof (AdornedControl),
                new FrameworkPropertyMetadata(true, IsMouseOverShowEnabled_PropertyChanged));

        public static readonly DependencyProperty FadeInTimeProperty = DependencyProperty.Register(
            "FadeInTime",
            typeof (double),
            typeof (AdornedControl),
            new FrameworkPropertyMetadata(0.25));

        public static readonly DependencyProperty FadeOutTimeProperty = DependencyProperty.Register(
            "FadeOutTime",
            typeof (double),
            typeof (AdornedControl),
            new FrameworkPropertyMetadata(1.0));

        public static readonly DependencyProperty CloseAdornerTimeOutProperty =
            DependencyProperty.Register(
                "CloseAdornerTimeOut",
                typeof (double),
                typeof (AdornedControl),
                new FrameworkPropertyMetadata(2.0, CloseAdornerTimeOut_PropertyChanged));

        public static readonly DependencyProperty AdornedTemplatePartNameProperty =
            DependencyProperty.Register(
                "AdornedTemplatePartName",
                typeof (string),
                typeof (AdornedControl),
                new FrameworkPropertyMetadata(null));

        #endregion Dependency Properties

        #region Commands

        /// <summary>
        ///     Commands.
        /// </summary>
        public static readonly RoutedCommand ShowAdornerCommand = new RoutedCommand(
            "ShowAdorner",
            typeof (AdornedControl));

        public static readonly RoutedCommand FadeInAdornerCommand = new RoutedCommand(
            "FadeInAdorner",
            typeof (AdornedControl));

        public static readonly RoutedCommand HideAdornerCommand = new RoutedCommand(
            "HideAdorner",
            typeof (AdornedControl));

        public static readonly RoutedCommand FadeOutAdornerCommand = new RoutedCommand(
            "FadeOutAdorner",
            typeof (AdornedControl));

        #endregion Commands

        #region Private Data Members

        /// <summary>
        ///     Command bindings.
        /// </summary>
        private static readonly CommandBinding ShowAdornerCommandBinding = new CommandBinding(
            ShowAdornerCommand,
            ShowAdornerCommand_Executed);

        private static readonly CommandBinding FadeInAdornerCommandBinding = new CommandBinding(
            FadeInAdornerCommand,
            FadeInAdornerCommand_Executed);

        private static readonly CommandBinding HideAdornerCommandBinding = new CommandBinding(
            HideAdornerCommand,
            HideAdornerCommand_Executed);

        private static readonly CommandBinding FadeOutAdornerCommandBinding = new CommandBinding(
            FadeInAdornerCommand,
            FadeOutAdornerCommand_Executed);

        /// <summary>
        ///     This timer is used to fade out and close the adorner.
        /// </summary>
        private readonly DispatcherTimer _closeAdornerTimer = new DispatcherTimer();

        /// <summary>
        ///     The actual adorner create to contain our 'adorner UI content'.
        /// </summary>
        private FrameworkElementAdorner _adorner;

        /// <summary>
        ///     Caches the adorner layer.
        /// </summary>
        private AdornerLayer _adornerLayer;

        /// <summary>
        ///     Specifies the current show/hide state of the adorner.
        /// </summary>
        private AdornerShowState _adornerShowState = AdornerShowState.Hidden;

        /// <summary>
        ///     Specifies the current show/hide state of the adorner.
        /// </summary>
        private enum AdornerShowState
        {
            Visible,

            Hidden,

            FadingIn,

            FadingOut
        }

        #endregion

        #region Private/Internal Functions

        /// <summary>
        ///     Static constructor to register command bindings.
        /// </summary>
        static AdornedControl()
        {
            CommandManager.RegisterClassCommandBinding(typeof (AdornedControl), ShowAdornerCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof (AdornedControl), FadeOutAdornerCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof (AdornedControl), HideAdornerCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof (AdornedControl), FadeInAdornerCommandBinding);
        }

        /// <summary>
        ///     Event raised when the DataContext of the adorned control changes.
        /// </summary>
        private void AdornedControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateAdornerDataContext();
        }

        /// <summary>
        ///     Update the DataContext of the adorner from the adorned control.
        /// </summary>
        private void UpdateAdornerDataContext()
        {
            if (AdornerContent != null)
            {
                AdornerContent.DataContext = DataContext;
            }
        }

        /// <summary>
        ///     Event raised when the Show command is executed.
        /// </summary>
        private static void ShowAdornerCommand_Executed(object target, ExecutedRoutedEventArgs e)
        {
            var c = (AdornedControl) target;
            c.ShowAdorner();
        }

        /// <summary>
        ///     Event raised when the FadeIn command is executed.
        /// </summary>
        private static void FadeInAdornerCommand_Executed(object target, ExecutedRoutedEventArgs e)
        {
            var c = (AdornedControl) target;
            c.FadeOutAdorner();
        }

        /// <summary>
        ///     Event raised when the Hide command is executed.
        /// </summary>
        private static void HideAdornerCommand_Executed(object target, ExecutedRoutedEventArgs e)
        {
            var c = (AdornedControl) target;
            c.HideAdorner();
        }

        /// <summary>
        ///     Event raised when the FadeOut command is executed.
        /// </summary>
        private static void FadeOutAdornerCommand_Executed(object target, ExecutedRoutedEventArgs e)
        {
            var c = (AdornedControl) target;
            c.FadeOutAdorner();
        }

        /// <summary>
        ///     Event raised when the value of IsAdornerVisible has changed.
        /// </summary>
        private static void IsAdornerVisible_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (AdornedControl) o;
            c.ShowOrHideAdornerInternal();
        }

        /// <summary>
        ///     Event raised when the IsMouseOverShowEnabled property has changed.
        /// </summary>
        private static void IsMouseOverShowEnabled_PropertyChanged(
            DependencyObject o,
            DependencyPropertyChangedEventArgs e)
        {
            var c = (AdornedControl) o;
            c._closeAdornerTimer.Stop();
            c.HideAdorner();
        }

        /// <summary>
        ///     Event raised when the CloseAdornerTimeOut property has change.
        /// </summary>
        private static void CloseAdornerTimeOut_PropertyChanged(
            DependencyObject o,
            DependencyPropertyChangedEventArgs e)
        {
            var c = (AdornedControl) o;
            c._closeAdornerTimer.Interval = TimeSpan.FromSeconds(c.CloseAdornerTimeOut);
        }

        /// <summary>
        ///     Event raised when the value of AdornerContent has changed.
        /// </summary>
        private static void AdornerContent_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (AdornedControl) o;
            c.ShowOrHideAdornerInternal();

            var oldAdornerContent = (FrameworkElement) e.OldValue;
            if (oldAdornerContent != null)
            {
                oldAdornerContent.MouseEnter -= c.AdornerContentMouseEnter;
                oldAdornerContent.MouseLeave -= c.AdornerContentMouseLeave;
            }

            var newAdornerContent = (FrameworkElement) e.NewValue;
            if (newAdornerContent != null)
            {
                newAdornerContent.MouseEnter += c.AdornerContentMouseEnter;
                newAdornerContent.MouseLeave += c.AdornerContentMouseLeave;
            }
        }

        /// <summary>
        ///     Event raised when the mouse cursor enters the area of the adorner.
        /// </summary>
        private void AdornerContentMouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnterLogic();
        }

        /// <summary>
        ///     Event raised when the mouse cursor leaves the area of the adorner.
        /// </summary>
        private void AdornerContentMouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeaveLogic();
        }

        /// <summary>
        ///     Internal method to show or hide the adorner based on the value of IsAdornerVisible.
        /// </summary>
        private void ShowOrHideAdornerInternal()
        {
            if (IsAdornerVisible)
            {
                ShowAdornerInternal();
            }
            else
            {
                HideAdornerInternal();
            }
        }

        /// <summary>
        ///     Finds a child element in the visual tree that has the specified name.
        ///     Returns null if no child with that name exists.
        /// </summary>
        public static FrameworkElement FindNamedChild(FrameworkElement rootElement, string childName)
        {
            var numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (var i = 0; i < numChildren; ++i)
            {
                var child = VisualTreeHelper.GetChild(rootElement, i);
                var childElement = child as FrameworkElement;
                if (childElement != null && childElement.Name == childName)
                {
                    return childElement;
                }

                var foundElement = FindNamedChild(childElement, childName);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        /// <summary>
        ///     Internal method to show the adorner.
        /// </summary>
        private void ShowAdornerInternal()
        {
            if (_adorner != null)
            {
                // Already adorned.
                return;
            }

            if (AdornerContent != null)
            {
                if (_adornerLayer == null)
                {
                    _adornerLayer = AdornerLayer.GetAdornerLayer(this);
                }

                if (_adornerLayer != null)
                {
                    FrameworkElement adornedControl = this; // The control to be adorned defaults to 'this'.

                    if (!string.IsNullOrEmpty(AdornedTemplatePartName))
                    {
                        //
                        // If 'AdornedTemplatePartName' is set to a valid string then search the visual-tree
                        // for a UI element that has the specified part name.  If we find it then use it as the
                        // adorned control, otherwise throw an exception.
                        //
                        adornedControl = FindNamedChild(this, AdornedTemplatePartName);
                        if (adornedControl == null)
                        {
                            throw new ApplicationException(
                                $"Failed to find a FrameworkElement in the visual-tree with the part name '{AdornedTemplatePartName}'.");
                        }
                    }

                    _adorner = new FrameworkElementAdorner(
                        AdornerContent,
                        adornedControl,
                        HorizontalAdornerPlacement,
                        VerticalAdornerPlacement,
                        AdornerOffsetX,
                        AdornerOffsetY);
                    _adornerLayer.Add(_adorner);

                    UpdateAdornerDataContext();
                }
            }

            _adornerShowState = AdornerShowState.Visible;
        }

        /// <summary>
        ///     Internal method to hide the adorner.
        /// </summary>
        private void HideAdornerInternal()
        {
            if (_adornerLayer == null || _adorner == null)
            {
                // Not already adorned.
                return;
            }

            //
            // Stop the timer that might be about to fade out the adorner.
            //
            _closeAdornerTimer.Stop();
            _adornerLayer.Remove(_adorner);
            _adorner.DisconnectChild();

            _adorner = null;
            _adornerLayer = null;

            //
            // Ensure that the state of the adorned control reflects that
            // the the adorner is no longer.
            //
            _adornerShowState = AdornerShowState.Hidden;
        }

        /// <summary>
        ///     Called to build the visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ShowOrHideAdornerInternal();
        }

        /// <summary>
        ///     Called when the mouse cursor enters the area of the adorned control.
        /// </summary>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            MouseEnterLogic();
        }

        /// <summary>
        ///     Called when the mouse cursor leaves the area of the adorned control.
        /// </summary>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            MouseLeaveLogic();
        }

        /// <summary>
        ///     Shared mouse enter code.
        /// </summary>
        private void MouseEnterLogic()
        {
            if (!IsMouseOverShowEnabled)
            {
                return;
            }

            _closeAdornerTimer.Stop();

            FadeInAdorner();
        }

        /// <summary>
        ///     Shared mouse leave code.
        /// </summary>
        private void MouseLeaveLogic()
        {
            if (!IsMouseOverShowEnabled)
            {
                return;
            }

            _closeAdornerTimer.Start();
        }

        /// <summary>
        ///     Called when the close adorner time-out has ellapsed, the mouse has moved
        ///     away from the adorned control and the adorner and it is time to close the adorner.
        /// </summary>
        private void CloseAdornerTimerTick(object sender, EventArgs e)
        {
            _closeAdornerTimer.Stop();

            FadeOutAdorner();
        }

        /// <summary>
        ///     Event raised when the fade in animation has completed.
        /// </summary>
        private void FadeInAnimationCompleted(object sender, EventArgs e)
        {
            _adornerShowState = AdornerShowState.Visible;
        }

        /// <summary>
        ///     Event raised when the fade-out animation has completed.
        /// </summary>
        private void FadeOutAnimationCompleted(object sender, EventArgs e)
        {
            if (_adornerShowState == AdornerShowState.FadingOut)
            {
                // Still fading out, eg it wasn't aborted.
                HideAdorner();
            }
        }

        #endregion
    }
}