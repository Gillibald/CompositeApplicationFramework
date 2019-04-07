#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IViewModelResolver<in TSource, out TTarget>
        where TTarget : IDisposable
    {
        /// <summary>
        ///     Resolves the view model.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        TTarget ResolveViewModel(TSource source);
    }
}