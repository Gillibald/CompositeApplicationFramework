#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CompositeApplicationFramework.Controls.Adorners;
using CompositeApplicationFramework.View;

#endregion

namespace CompositeApplicationFramework.Controls
{
    /// <summary>
    ///     Führen Sie die Schritte 1a oder 1b und anschließend Schritt 2 aus, um dieses benutzerdefinierte Steuerelement in
    ///     einer XAML-Datei zu verwenden.
    ///     Schritt 1a) Verwenden des benutzerdefinierten Steuerelements in einer XAML-Datei, die im aktuellen Projekt
    ///     vorhanden ist.
    ///     Fügen Sie dieses XmlNamespace-Attribut dem Stammelement der Markupdatei
    ///     an der Stelle hinzu, an der es verwendet werden soll:
    ///     xmlns:MyNamespace="clr-namespace:WpfBase.Controls"
    ///     Schritt 1b) Verwenden des benutzerdefinierten Steuerelements in einer XAML-Datei, die in einem anderen Projekt
    ///     vorhanden ist.
    ///     Fügen Sie dieses XmlNamespace-Attribut dem Stammelement der Markupdatei
    ///     an der Stelle hinzu, an der es verwendet werden soll:
    ///     xmlns:MyNamespace="clr-namespace:WpfBase.Controls;assembly=WpfBase.Controls"
    ///     Darüber hinaus müssen Sie von dem Projekt, das die XAML-Datei enthält, einen Projektverweis
    ///     zu diesem Projekt hinzufügen und das Projekt neu erstellen, um Kompilierungsfehler zu vermeiden:
    ///     Klicken Sie im Projektmappen-Explorer mit der rechten Maustaste auf das Zielprojekt und anschließend auf
    ///     "Verweis hinzufügen"->"Projekte"->[Navigieren Sie zu diesem Projekt, und wählen Sie es aus.]
    ///     Schritt 2)
    ///     Fahren Sie fort, und verwenden Sie das Steuerelement in der XAML-Datei.
    ///     <MyNamespace:BusyIndicator />
    /// </summary>
    [TemplatePart(Name = "PART_AdornedContainer", Type = typeof (AdornedControl))]
    public class BusyIndicator : ContentControl
    {
        public static readonly DependencyProperty IsBusyProperty;
        public static readonly DependencyProperty BusyTextProperty;
        public static readonly DependencyProperty IndicatorColorProperty;

        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (BusyIndicator),
                new FrameworkPropertyMetadata(typeof (BusyIndicator)));
            IsBusyProperty = DependencyProperty.Register(
                "IsBusy",
                typeof (bool),
                typeof (BusyIndicator),
                new UIPropertyMetadata(false, OnIsBusyPropertyChanged));
            BusyTextProperty = DependencyProperty.Register(
                "BusyText",
                typeof (string),
                typeof (BusyIndicator),
                new UIPropertyMetadata("Loading...", OnLoadingTextPropertyChanged));
            IndicatorColorProperty = DependencyProperty.Register(
                "IndicatorColor",
                typeof (SolidColorBrush),
                typeof (BusyIndicator),
                new UIPropertyMetadata(Brushes.CornflowerBlue, OnIndicatorColorPropertyChanged));
        }

        public DispatcherTimer AnimationTimer { get; set; }
        public AdornedControl AdornedContainer { get; internal set; }
        public DependencyObject AssociatedObject { get; set; }

        protected override void OnInitialized(EventArgs e)
        {
            AnimationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher)
            {
                Interval = new TimeSpan(0, 0, 0, 0, 75)
            };
            base.OnInitialized(e);
        }

        public override void OnApplyTemplate()
        {
            AdornedContainer = GetTemplateChild("PART_AdornedContainer") as AdornedControl;
            if (AdornedContainer != null)
            {
                AdornedContainer.AdornerContent = new LoadingWait();
                AdornedContainer.IsAdornerVisible = IsBusy;
                AdornedContainer.AdornerContent.ToolTip = BusyText;
            }
            base.OnApplyTemplate();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            AssociatedObject = newContent as DependencyObject;

            AssociatedObject?.SetValue(IsEnabledProperty, IsBusy);

            var lOldContent = oldContent as DependencyObject;
            if ((bool?) lOldContent?.GetValue(IsEnabledProperty) == false)
            {
                lOldContent.SetValue(IsEnabledProperty, true);
            }

            base.OnContentChanged(oldContent, newContent);
        }

        #region DependencyProperties

        public bool IsBusy
        {
            get => (bool) GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public string BusyText
        {
            get => (string) GetValue(BusyTextProperty);
            set => SetValue(BusyTextProperty, value);
        }

        public SolidColorBrush IndicatorColor
        {
            get => (SolidColorBrush) GetValue(IndicatorColorProperty);
            set => SetValue(IndicatorColorProperty, value);
        }

        private static void OnIsBusyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var lIndicator = sender as BusyIndicator;
            var lIsBusy = (bool) e.NewValue;
            if (lIndicator?.AssociatedObject == null || lIndicator.AdornedContainer == null)
            {
                return;
            }
            lIndicator.AssociatedObject.SetValue(IsEnabledProperty, !lIsBusy);
            lIndicator.AdornedContainer.IsAdornerVisible = lIsBusy;
        }

        private static void OnLoadingTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnIndicatorColorPropertyChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion
    }
}