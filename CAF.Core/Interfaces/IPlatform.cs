namespace CompositeApplicationFramework.Interfaces
{
    public interface IPlatform
    {
        void SetBindingContext(object view, IViewModel context);

        /// <summary>
        ///     Is component visible?
        ///     null = hiddend
        ///     true = Visible
        ///     false = Collapsed
        /// </summary>
        void SetVisibilityState(object view, bool? visibilityState);
    }
}
