using System;

namespace CompositeApplicationFramework.Interfaces
{
    public interface INavigationResult
    {
        bool Success { get; }

        Exception Exception { get; }
    }
}
