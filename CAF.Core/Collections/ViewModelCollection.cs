#region Usings

using System;
using System.Collections.Generic;
using CompositeApplicationFramework.Interfaces;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Collections
{
    public class ViewModelCollection<TSource, TTarget> : TransformedCollection<TSource, TTarget>,
        IViewModelCollection<TSource, TTarget>
        where TTarget : IDisposable
    {
        public ViewModelCollection([NotNull] IEnumerable<TSource> models,
            [NotNull] IViewModelResolver<TSource, TTarget> resolver)
            : base(models, resolver.ResolveViewModel, actionViewModel => actionViewModel.Dispose())
        {
        }

        IEnumerator<TTarget> IEnumerable<TTarget>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}