#region Usings

using System.Reflection;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Interfaces;
using Unity;

#endregion

namespace CompositeApplicationFramework.Module
{
    using System.Threading.Tasks;

    public class SingleViewModule<T> : ModuleBase
        where T : IPresenter
    {
        /// <summary>
        ///     Shell
        /// </summary>
        [Dependency]
        public IShell Shell { get; set; }

        /// <summary>
        ///     Presenter
        /// </summary>
        [Dependency]
        public T Presenter { get; set; }


        protected override async Task InitializeViewsAsync()
        {
            if (!(typeof (T).GetCustomAttribute(typeof (RegionAttribute)) is RegionAttribute attr))
            {
                return;
            }

            await AddPresenterAsync(Presenter);

            Shell.AddViewToRegion(GetPresenter<T>().View, attr.Region);

            await base.InitializeViewsAsync();
        }
    }
}