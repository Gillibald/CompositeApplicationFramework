namespace CompositeApplicationFramework.Interfaces
{
    /// <summary>
    ///     Shell interface
    /// </summary>
    public interface IShell
    {
        /// <summary>
        ///     Bootstrapper
        /// </summary>
        IBootstrapper Bootstrapper { get; set; }

        bool AddViewToRegion(object view, string region);
    }
}