namespace CompositeApplicationFramework.Interfaces
{
    public interface IChildDto<TKey> : IDto
    {
        /// <summary>
        ///     Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        ///     The parent identifier.
        /// </value>
        TKey ParentId { get; set; }
    }
}