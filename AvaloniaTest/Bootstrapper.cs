using AvaloniaTest.View;
using AvaloniaTest.ViewModel;
using CompositeApplicationFramework.Common;
using CompositeApplicationFramework.Module;
using CompositeApplicationFramework.Presenter;

namespace AvaloniaTest
{
    class Bootstrapper : AvaloniaBootstrapper
    {
        protected override void CreateModuleCatalog()
        {
            base.CreateModuleCatalog();

            ModuleCatalog.Add(typeof(SingleViewModule<MvpVmPresenter<HomeView, HomeViewModel>>));
        }
    }
}
