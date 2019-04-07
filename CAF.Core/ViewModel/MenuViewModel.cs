#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CompositeApplicationFramework.Commands;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Model;
using CompositeApplicationFramework.Types;
using JetBrains.Annotations;
using Unity;
using NavigationCommands = CompositeApplicationFramework.Commands.NavigationCommands;

#endregion

namespace CompositeApplicationFramework.ViewModel
{
    using System;
    using System.Threading.Tasks;
    using Helper;

    /// <summary>
    ///     Shell view model
    /// </summary>
    [UsedImplicitly]
    public class MenuViewModel : MvpVmViewModel, IMenuViewModel
    {
        private IEnumerable<MenuEntry> _menus;
        private DelegateCommand<object> _navigateToViewCommand;
        private MenuEntry _selectedMenu;

        private ICommand NavigateToViewCommand
            => _navigateToViewCommand ?? (_navigateToViewCommand = new DelegateCommand<object>(NavigateToView));

        [Dependency]
        public INavigationService NavigationService { get; set; }

        /// <summary>
        ///     Menus
        /// </summary>
        public IEnumerable<MenuEntry> MenuEntries
        {
            get => _menus;
            set
            {
                _menus = value;

                RaisePropertyChanged(() => MenuEntries);

                if (value != null && value.Any())
                {
                    SelectedMenuEntry = value.First();
                }
            }
        }

        /// <summary>
        ///     Selected Menu item
        /// </summary>
        public MenuEntry SelectedMenuEntry
        {
            get => _selectedMenu;
            set
            {
                _selectedMenu = value;

                Logger.Info("PUBLISH:<ProcessEvent>(MenuSelected): {0}.SelectedMenu: {1} ({2})", ClassName, value.Name,
                    Id);

                // Publish menu selected event
                EventAggregator.GetEvent<ProcessEvent>()
                    .Publish(new ProcessEventArgs { Id = Id, Data = value, ProcessType = ProcessType.MenuSelected });

                NavigationService.NavigateAsync(NavigationHelper.GetNameByModuleType(value.ModuleType));

                RaisePropertyChanged(() => SelectedMenuEntry);
            }
        }
        
        private async void NavigateToView(object viewType)
        {
            var target = NavigationHelper.GetNameByViewType((Type)viewType);

            await NavigationService.NavigateAsync(target);
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            NavigationCommands.NavigateToViewCommand.RegisterCommand(NavigateToViewCommand);

            Logger.Info("PUBLISH:<GlobalCommand>(Registered): {0}: {1} ({2})", ClassName, "NavigateToViewCommand", Id);

            NavigationService.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Target))
            {
                return;
            }

            var menuEntry = MenuEntries.SingleOrDefault(x => NavigationHelper.GetNameByModuleType(x.ModuleType) == e.Target);

            if (menuEntry == null)
            {
                return;
            }

            if (SelectedMenuEntry == menuEntry) return;

            SelectedMenuEntry = menuEntry;
        }
    }
}