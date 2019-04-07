#region Usings

using System.Collections.ObjectModel;
using System.Linq;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Model;

#endregion

namespace CompositeApplicationFramework.Presenter
{
    #region Dependencies

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class MenuPresenter<TView, TViewModel> : MvpVmPresenter<TView, TViewModel> where TViewModel : class, IMenuViewModel
    {
        /// <summary>
        ///     When all modules are initialized get a list of modules that
        ///     are menus and add them to the shell's view model
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ModulesInitialized(IModuleCatalog moduleCatalog)
        {
            base.ModulesInitialized(moduleCatalog);

            // Get list of modules that are menu items
            var query = (from m in moduleCatalog.Modules
                select
                    new MenuEntry
                    {
                        Id = m.Id.ToString().Replace("-", ""),
                        Name = m.Name,
                        ModuleType = m.GetType()
                    }).ToList();

            // List of menus
            ViewModel.MenuEntries = new ObservableCollection<MenuEntry>(query);
        }
    }
}