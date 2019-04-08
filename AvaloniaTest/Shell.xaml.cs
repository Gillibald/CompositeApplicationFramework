using System.Collections;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using CompositeApplicationFramework.Interfaces;

namespace AvaloniaTest
{
    public class Shell : Window, IShell
    {
        public Shell()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public IBootstrapper Bootstrapper { get; set; }

        public bool AddViewToRegion(object view, string region)
        {
            var itemsControl = this.Find<ItemsControl>(region);

            if (itemsControl == null)
            {
                return false;
            }

            if (!(itemsControl.Items is IList items))
            {
                items = new AvaloniaList<object>();

                itemsControl.Items = items;
            }

            items.Add(view);

            if (view is Control control)
            {
                control.IsVisible = false;
            }

            return true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
