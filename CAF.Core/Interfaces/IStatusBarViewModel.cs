namespace CompositeApplicationFramework.Interfaces
{
    public interface IStatusBarViewModel : IViewModel
    {
        /// <summary>
        ///     Status bar message
        /// </summary>
        string StatusBarMessage { get; set; }
    }
}