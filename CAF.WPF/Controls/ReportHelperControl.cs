﻿#region Usings

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CompositeApplicationFramework.Interfaces;

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
    ///     xmlns:MyNamespace="clr-namespace:WpfBase"
    ///     Schritt 1b) Verwenden des benutzerdefinierten Steuerelements in einer XAML-Datei, die in einem anderen Projekt
    ///     vorhanden ist.
    ///     Fügen Sie dieses XmlNamespace-Attribut dem Stammelement der Markupdatei
    ///     an der Stelle hinzu, an der es verwendet werden soll:
    ///     xmlns:MyNamespace="clr-namespace:WpfBase;assembly=WpfBase"
    ///     Darüber hinaus müssen Sie von dem Projekt, das die XAML-Datei enthält, einen Projektverweis
    ///     zu diesem Projekt hinzufügen und das Projekt neu erstellen, um Kompilierungsfehler zu vermeiden:
    ///     Klicken Sie im Projektmappen-Explorer mit der rechten Maustaste auf das Zielprojekt und anschließend auf
    ///     "Verweis hinzufügen"->"Projekte"->[Navigieren Sie zu diesem Projekt, und wählen Sie es aus.]
    ///     Schritt 2)
    ///     Fahren Sie fort, und verwenden Sie das Steuerelement in der XAML-Datei.
    ///     <MyNamespace:ReportHelperControl />
    /// </summary>
    public class ReportHelperControl : Control
    {
        static ReportHelperControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (ReportHelperControl),
                new FrameworkPropertyMetadata(typeof (ReportHelperControl)));

            ItemsView.GroupDescriptions?.Add(new PropertyGroupDescription {PropertyName = "Category"});
        }

        public static ObservableCollection<IReportItem> Items { get; internal set; } =
            new ObservableCollection<IReportItem>();

        public static ListCollectionView ItemsView { get; internal set; } = new ListCollectionView(Items);

        public static void AddItem(IReportItem pItem)
        {
            if (!Items.Contains(pItem))
            {
                Items.Add(pItem);
            }
        }

        public static void RemoveItem(IReportItem pItem)
        {
            if (Items.Contains(pItem))
            {
                Items.Remove(pItem);
            }
        }
    }
}