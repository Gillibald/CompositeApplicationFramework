using Avalonia;
using Avalonia.Logging.Serilog;

namespace AvaloniaTest
{
    class Program
    {
        private static Bootstrapper _bootstrapper;

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static async void AppMain(Application app, string[] args)
        {
            _bootstrapper = new Bootstrapper();

            var shell = new Shell();        

            app.Run(shell);

            await _bootstrapper.RunAsync(shell);
        }
    }
}
