using Avalonia;
using Avalonia.Markup.Xaml;

namespace AvaloniaTest
{
    public class App : Application
    {
        private Bootstrapper _bootstrapper;

        public override async void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            _bootstrapper = new Bootstrapper();

            await _bootstrapper.RunAsync();
        }
    }
}
