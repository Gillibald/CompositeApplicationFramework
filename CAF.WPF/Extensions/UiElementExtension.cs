using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CompositeApplicationFramework.Extensions
{
    public static class UiElementExtension
    {
        public static void DelayedFocus(this UIElement pElement)
        {
            pElement.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        pElement.Focusable = true;
                        pElement.Focus();
                        Keyboard.Focus(pElement);
                    }),
                DispatcherPriority.Render);
        }
    }
}