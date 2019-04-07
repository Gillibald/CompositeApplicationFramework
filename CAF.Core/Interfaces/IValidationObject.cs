namespace CompositeApplicationFramework.Interfaces
{
    public interface IValidationObject
    {
        /// <summary>
        ///     Gets or sets the assigned object.
        /// </summary>
        /// <value>
        ///     The assigned object.
        /// </value>
        object AssignedObject { get; set; }

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <value>
        ///     The name of the property.
        /// </value>
        string PropertyName { get; }

        /// <summary>
        ///     Gets or sets the error message.
        /// </summary>
        /// <value>
        ///     The error message.
        /// </value>
        string ErrorMessage { get; set; }

        /// <summary>
        ///     Validates this instance.
        /// </summary>
        /// <returns></returns>
        bool Validate();
    }
}