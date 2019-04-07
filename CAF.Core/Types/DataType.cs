namespace CompositeApplicationFramework.Types
{
    /// <summary>
    ///     Data event type
    /// </summary>
    public enum DataType
    {
        /// <summary>
        ///     The not defined is default
        /// </summary>
        NotDefined,

        /// <summary>
        ///     Create
        /// </summary>
        Create,

        /// <summary>
        ///     Read
        /// </summary>
        Read,

        /// <summary>
        ///     Update
        /// </summary>
        Update,

        /// <summary>
        ///     Delete
        /// </summary>
        Delete,

        /// <summary>
        ///     List
        /// </summary>
        List,

        /// <summary>
        ///     Stream
        /// </summary>
        Stream,

        /// <summary>
        ///     The service result
        /// </summary>
        ServiceResult,

        /// <summary>
        ///     The process data
        /// </summary>
        ProcessData,

        /// <summary>
        ///     The file
        /// </summary>
        File,

        /// <summary>
        ///     The multiple files
        /// </summary>
        MultipleFiles
    }
}