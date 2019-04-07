namespace CompositeApplicationFramework.Interfaces
{
    public interface IReportEntry
    {
        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <value>
        ///     The name of the property.
        /// </value>
        string PropertyName { get; }

        /// <summary>
        ///     Gets the assigned object.
        /// </summary>
        /// <value>
        ///     The assigned object.
        /// </value>
        object AssignedObject { get; }

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        /// <value>
        ///     The header.
        /// </value>
        string Header { get; set; }

        /// <summary>
        ///     Gets or sets the tooltip.
        /// </summary>
        /// <value>
        ///     The tooltip.
        /// </value>
        string Tooltip { get; set; }
    }
}