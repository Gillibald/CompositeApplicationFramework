#region Usings

using System.Collections.Generic;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IReportItem
    {
        /// <summary>
        ///     Gets the entries.
        /// </summary>
        /// <value>
        ///     The entries.
        /// </value>
        IEnumerable<IReportEntry> Entries { get; }

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

        /// <summary>
        ///     Gets or sets the category.
        /// </summary>
        /// <value>
        ///     The category.
        /// </value>
        string Category { get; set; }

        /// <summary>
        ///     Adds the entry.
        /// </summary>
        /// <param name="entry">The p entry.</param>
        void AddEntry(IReportEntry entry);

        /// <summary>
        ///     Removes the entry.
        /// </summary>
        /// <param name="entry">The p entry.</param>
        void RemoveEntry(IReportEntry entry);
    }
}