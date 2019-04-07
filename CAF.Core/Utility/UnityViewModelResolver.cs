using Unity;

namespace CompositeApplicationFramework.Utility
{
    using Interfaces;
    using JetBrains.Annotations;

    public class UnityViewModelResolver<TSource, TTarget> : IViewModelResolver<TSource, TTarget> where TTarget : IViewModel<TSource>
    {
        private readonly IUnityContainer _container;
        private readonly IViewModel _parent;

        public UnityViewModelResolver([NotNull] IUnityContainer container)
        {
            _container = container;
        }

        public UnityViewModelResolver([NotNull] IUnityContainer container, IViewModel parent) : this(container)
        {
            _parent = parent;
        }

        public TTarget ResolveViewModel(TSource source)
        {
            var viewModel = _container.Resolve<TTarget>();
            SetupViewModel(viewModel, source);
            return viewModel;
        }

        private async void SetupViewModel(IViewModel<TSource> viewModel, TSource source)
        {
            viewModel.ParentViewModel = _parent;
            viewModel.Model = source;
            await viewModel.InitializeAsync();
        }
    }
}