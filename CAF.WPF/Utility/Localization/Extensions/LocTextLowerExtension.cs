#region Usings

using System.Windows.Markup;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Extensions
{
    [MarkupExtensionReturnType(typeof (string))]
    public class LocTextLowerExtension : LocTextExtension
    {
        #region Text Formatting

        /// <summary>
        ///     This method formats the localized text.
        ///     If the passed target text is null, string.empty will be returned.
        /// </summary>
        /// <param name="target">The text to format.</param>
        /// <returns>
        ///     Returns the formated text or string.empty, if the target text was null.
        /// </returns>
        protected override string FormatText(string target)
        {
            return target == null ? string.Empty : target.ToLower(GetForcedCultureOrDefault());
        }

        #endregion

        #region Constructors

        public LocTextLowerExtension()
            : base()
        {
        }

        public LocTextLowerExtension(string key)
            : base(key)
        {
        }

        #endregion
    }
}