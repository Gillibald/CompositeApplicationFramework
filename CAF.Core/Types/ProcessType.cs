namespace CompositeApplicationFramework.Types
{
    /// <summary>
    ///     MvpVm framework processes
    /// </summary>
    public enum ProcessType
    {
        /// <summary> Status bar message</summary>
        StatusBarMessage,

        /// <summary> System message</summary>
        SystemMessage,

        /// <summary> Module Added</summary>
        ModuleAdded,

        /// <summary> Menu items selected</summary>
        MenuSelected,

        /// <summary> Property in ViewModel has changed</summary>
        ViewModelChanged,

        /// <summary> Selection in ViewModel has changed</summary>
        SelectionChanged
    }
}