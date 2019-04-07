using CompositeApplicationFramework.Interfaces;

namespace CompositeApplicationFramework.Events
{
    public class NavigationEventArgs
    {
        public NavigationEventArgs(string source, string target, INavigationParameters parameters)
        {
            Source = source;
            Target = target;
            Parameters = parameters;
        }

        public string Target { get; }
        public string Source { get; }
        public INavigationParameters Parameters { get; }
    }
}
