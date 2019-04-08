using Avalonia;
using Avalonia.Controls;
using AvaloniaTest.View;
using AvaloniaTest.ViewModel;
using CompositeApplicationFramework.Common;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Module;
using CompositeApplicationFramework.Presenter;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace AvaloniaTest
{
    class Bootstrapper : AvaloniaBootstrapper
    {
        protected override void CreateModuleCatalog()
        {
            base.CreateModuleCatalog();

            ModuleCatalog.Add(typeof(SingleViewModule<MvpVmPresenter<HomeView, HomeViewModel>>));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IShell, Shell>(new ContainerControlledLifetimeManager());
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var mainWindow = (Window)Shell;

            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();

            await NavigationService.NavigateAsync("Home");
        }
    }
}
