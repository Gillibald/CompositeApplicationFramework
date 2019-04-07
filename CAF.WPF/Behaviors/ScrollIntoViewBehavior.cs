﻿#region Usings

using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

#endregion

namespace CompositeApplicationFramework.Behaviors
{
    public class ScrollIntoViewBehavior : Behavior<ListBox>
    {
        /// <summary>
        ///  When Beahvior is attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObjectSelectionChanged;
        }

        /// <summary>
        /// On Selection Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem != null)
            {
                listBox.Dispatcher.BeginInvoke(
                    (Action) (() =>
                    {
                        listBox.UpdateLayout();
                        if (listBox.SelectedItem !=
                            null)
                            listBox.ScrollIntoView(
                                listBox.SelectedItem);
                    }));
            }
        }

        /// <summary>
        /// When behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -=
                AssociatedObjectSelectionChanged;
        }
    }
}