#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Model
{
    /// <summary>
    /// </summary>
    public class MenuEntry
    {
        /// <summary>
        /// </summary>
        public Type ModuleType { get; set; }

        /// <summary>
        ///     Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Id
        /// </summary>
        public string Id { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}