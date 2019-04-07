using System;
using CompositeApplicationFramework.Interfaces;

namespace CompositeApplicationFramework.Navigation
{
    public class NavigationResult : INavigationResult
    {
        public bool Success { get; set; }

        public Exception Exception { get; set; }
    }
}