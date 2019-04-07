﻿#region Usings

using System.Linq;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Providers
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     A class that bundles the key, assembly and dictionary information.
    /// </summary>
    public class FQAssemblyDictionaryKey : FullyQualifiedResourceKeyBase
    {
        /// <summary>
        ///     Creates a new instance of <see cref="FullyQualifiedResourceKeyBase" />.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="assembly">The assembly of the dictionary.</param>
        /// <param name="dictionary">The resource dictionary.</param>
        public FQAssemblyDictionaryKey(string key, string assembly = null, string dictionary = null)
        {
            Key = key;
            Assembly = assembly;
            Dictionary = dictionary;
        }

        /// <summary>
        ///     The key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     The assembly of the dictionary.
        /// </summary>
        public string Assembly { get; }

        /// <summary>
        ///     The resource dictionary.
        /// </summary>
        public string Dictionary { get; }

        /// <summary>
        ///     Converts the object to a string.
        /// </summary>
        /// <returns>The joined version of the assembly, dictionary and key.</returns>
        public override string ToString()
        {
            return string.Join(
                ":",
                (new[] {Assembly, Dictionary, Key}).Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }
    }
}