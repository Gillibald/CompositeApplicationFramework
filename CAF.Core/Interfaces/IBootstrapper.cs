#region Usings

using Unity;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Bootstrapper interface
    /// </summary>
    public interface IBootstrapper
    {
        IShell Shell { get; }

        /// <summary>
        ///     Unity container
        /// </summary>
        IUnityContainer Container { get; }

        /// <summary>
        ///     Run command
        /// </summary>
        Task RunAsync();

        /// <summary>
        ///     Run command
        /// </summary>
        Task RunAsync(IUnityContainer container);

        /// <summary>
        ///     Run command with shell parameter
        /// </summary>
        /// <param name="shell"></param>
        Task RunAsync(IShell shell);

        /// <summary>
        ///     Run command with container and shell parameter
        /// </summary>
        /// <param name="container"></param>
        /// <param name="shell"></param>
        Task RunAsync(IUnityContainer container, IShell shell);
    }
}