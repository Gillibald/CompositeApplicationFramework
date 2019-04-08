using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.ViewModel;
using System;

namespace AvaloniaTest.ViewModel
{
    class HomeViewModel : MvpVmViewModel, IConfirmNavigation
    {
        protected override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            Console.WriteLine("Navigating to Home");
        }

        protected override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Console.WriteLine("Navigated to Home");
        }

        public bool CanNavigate(INavigationParameters parameters)
        {
            Console.WriteLine("Confirm navigation from Home");

            return true;
        }
    }
}
