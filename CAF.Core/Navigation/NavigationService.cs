using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Helper;
using CompositeApplicationFramework.Interfaces;
using JetBrains.Annotations;
using Unity;

namespace CompositeApplicationFramework.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<string> _navigationStack = new Stack<string>();

        [Dependency]
        [UsedImplicitly]
        public IModuleCatalog ModuleCatalog { get; set; }
        [Dependency]
        [UsedImplicitly]
        public IPlatform Platform { get; set; }

        public event EventHandler<NavigationEventArgs> Navigated;

        public Task<INavigationResult> GoBackAsync()
        {
            if (!_navigationStack.Any())
            {
                return Task.FromResult<INavigationResult>(new NavigationResult { Success = false });
            }

            var viewName = _navigationStack.Pop();

            var presenters =
                ModuleCatalog.Modules.SelectMany(x => x.Presenters);

            foreach (var presenter in presenters)
            {
                Platform.SetVisibilityState(presenter.View, GetViewName(presenter.View) == viewName);
            }

            return Task.FromResult<INavigationResult>(new NavigationResult { Success = true });
        }

        public Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
        {
            return GoBackAsync();
        }

        public Task<INavigationResult> NavigateAsync(Uri uri)
        {
            return NavigateAsync(uri, new NavigationParameters());
        }

        public async Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
        {
            var navigationSegments = UriParsingHelper.GetUriSegments(uri);

            foreach (var navigationSegment in navigationSegments)
            {
                var segnentParameters = UriParsingHelper.GetSegmentParameters(navigationSegment, parameters);

                var segmentName = UriParsingHelper.GetSegmentName(navigationSegment);

                var navigationResult = await NavigateAsync(segmentName, segnentParameters);

                if (!navigationResult.Success)
                {
                    return new NavigationResult { Exception = new InvalidOperationException("Target not found."), Success = false };
                }
            }

            return new NavigationResult { Success = true };
        }

        public Task<INavigationResult> NavigateAsync(string name)
        {
            return NavigateAsync(name, new NavigationParameters());
        }

        public async Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters)
        {
            var lastNavigationStackEntry = _navigationStack.LastOrDefault();

            var sourcePresenter = ModuleCatalog.Modules.SelectMany(x => x.Presenters)
                .SingleOrDefault(x => GetViewName(x.View) == lastNavigationStackEntry);

            if (!await ExecuteConfirmNavigationAsync(sourcePresenter, parameters))
            {
                return new NavigationResult
                {
                    Success = false,
                    Exception = new InvalidOperationException("View not found.")
                };
            }

            var targetPresenter = ModuleCatalog.Modules.SelectMany(x => x.Presenters)
                .SingleOrDefault(x => GetViewName(x.View) == name);

            if (targetPresenter == null)
                return new NavigationResult
                {
                    Success = false,
                    Exception = new InvalidOperationException("Target not found.")
                };

            var region = targetPresenter.GetType().GetCustomAttribute<RegionAttribute>().Region;

            ExecuteOnNavigating(targetPresenter, parameters);

            var presenters = ModuleCatalog.Modules.SelectMany(x => x.Presenters);

            foreach (var presenter in presenters)
            {
                var regionAttribute = presenter.GetType().GetCustomAttribute<RegionAttribute>();
                if (regionAttribute.Region != region) continue;
                Platform.SetVisibilityState(presenter.View, false);
            }

            _navigationStack.Push(name);

            Platform.SetVisibilityState(targetPresenter.View, true);

            ExecuteOnNavigatedTo(targetPresenter, parameters);

            ExecuteOnNavigatedFrom(sourcePresenter, parameters);

            RaiseNavigated(sourcePresenter, targetPresenter, parameters);

            return new NavigationResult
            {
                Success = true,
            };
        }

        private void RaiseNavigated(IPresenter sourcePresenter, IPresenter targetPresenter, INavigationParameters parameters)
        {
            var source = string.Empty;

            if (sourcePresenter != null)
            {
                source = NavigationHelper.GetNameByPresenter(sourcePresenter);
            }
            
            var target = NavigationHelper.GetNameByPresenter(targetPresenter);

            Navigated?.Invoke(this, new NavigationEventArgs(source, target, parameters));
        }

        private static async Task<bool> ExecuteConfirmNavigationAsync(IPresenter presenter, INavigationParameters parameters)
        {
            if (presenter == null) return true;
            var view = presenter.View;
            var viewModel = presenter.ViewModel;
            return await ExecuteConfirmNavigationAsyncIfAvailable(presenter, parameters) &&
                   await ExecuteConfirmNavigationAsyncIfAvailable(view, parameters) &&
                   await ExecuteConfirmNavigationAsyncIfAvailable(viewModel, parameters);
        }

        private static void ExecuteOnNavigating(IPresenter presenter, INavigationParameters parameters)
        {
            if (presenter == null) return;
            var view = presenter.View;
            var viewModel = presenter.ViewModel;
            ExecuteOnNavigatingIfAvailable(presenter, parameters);
            ExecuteOnNavigatingIfAvailable(view, parameters);
            ExecuteOnNavigatingIfAvailable(viewModel, parameters);
        }

        private static void ExecuteOnNavigatingIfAvailable(object target, INavigationParameters parameters)
        {
            if (target is INavigatingAware confirmNavigationAsync)
            {
                confirmNavigationAsync.OnNavigatingTo(parameters);
            }
        }

        private static void ExecuteOnNavigatedTo(IPresenter presenter, INavigationParameters parameters)
        {
            if (presenter == null) return;
            var view = presenter.View;
            var viewModel = presenter.ViewModel;
            ExecuteOnNavigatedToIfAvailable(presenter, parameters);
            ExecuteOnNavigatedToIfAvailable(view, parameters);
            ExecuteOnNavigatedToIfAvailable(viewModel, parameters);
        }

        private static void ExecuteOnNavigatedToIfAvailable(object target, INavigationParameters parameters)
        {
            if (target is INavigatedAware confirmNavigationAsync)
            {
                confirmNavigationAsync.OnNavigatedTo(parameters);
            }
        }

        private static void ExecuteOnNavigatedFrom(IPresenter presenter, INavigationParameters parameters)
        {
            if (presenter == null) return;
            var view = presenter.View;
            var viewModel = presenter.ViewModel;
            ExecuteOnNavigatedFromIfAvailable(presenter, parameters);
            ExecuteOnNavigatedFromIfAvailable(view, parameters);
            ExecuteOnNavigatedFromIfAvailable(viewModel, parameters);
        }

        private static void ExecuteOnNavigatedFromIfAvailable(object target, INavigationParameters parameters)
        {
            if (target is INavigatedAware confirmNavigationAsync)
            {
                confirmNavigationAsync.OnNavigatedFrom(parameters);
            }
        }

        private static Task<bool> ExecuteConfirmNavigationAsyncIfAvailable(object target, INavigationParameters parameters)
        {
            if (target is IConfirmNavigationAsync confirmNavigationAsync)
            {
                return confirmNavigationAsync.CanNavigateAsync(parameters);
            }

            return Task.FromResult(true);
        }

        private static string GetViewName(object view)
        {
            return view == null ? string.Empty : view.GetType().Name.Replace("View", "");
        }
    }
}
