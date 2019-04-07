#region Usings

using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class SimpleViewModelResolver<TSource, TTarget> : IViewModelResolver<TSource, TTarget>
        where TTarget : IViewModel<TSource>
    {
        private readonly IViewModel _parent;

        public SimpleViewModelResolver()
        {

        }

        public SimpleViewModelResolver(IViewModel parent)
        {
            _parent = parent;
        }

        public TTarget ResolveViewModel(TSource source)
        {
            var viewModel = FastActivator.CreateInstance<TTarget>();
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