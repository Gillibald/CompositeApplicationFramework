using System;

namespace CompositeApplicationFramework.Helper
{
    using Interfaces;
    using JetBrains.Annotations;

    public static class NavigationHelper
    {
        public static string GetNameByModuleType(Type moduleType)
        {
            return moduleType.Name.Replace("Module", "");
        }

        public static string GetNameByModule([NotNull] IModule module)
        {
            return GetNameByModuleType(module.GetType());
        }

        public static string GetNameByViewModelType(Type viewModelType)
        {
            return viewModelType.Name.Replace("ViewModel", "");
        }

        public static string GetNameViewModel([NotNull] IViewModel viewModel)
        {
            return GetNameByViewModelType(viewModel.GetType());
        }

        public static string GetNameByPresenterType(Type presenterType)
        {
            return presenterType.Name.Replace("Presenter", "");
        }

        public static string GetNameByPresenter([NotNull] IPresenter presenter)
        {
            return GetNameByPresenterType(presenter.GetType());
        }

        public static string GetNameByViewType(Type viewType)
        {
            return viewType.Name.Replace("View", "");
        }

        public static string GetNameByView([NotNull] object view)
        {
            return GetNameByViewType(view.GetType());
        }
    }
}
