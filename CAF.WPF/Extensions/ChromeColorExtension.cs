#region Usings

using System;
using System.Windows.Markup;
using System.Windows.Media;
using CompositeApplicationFramework.Helper;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    #region Dependencies

    

    #endregion

    [MarkupExtensionReturnType(typeof (Color))]
    public class ChromeColorExtension : MarkupExtension
    {
        /// <summary>
        ///     When implemented in a derived class, returns an object that is set as the value of the target property for this
        ///     markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        ///     The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return WindowHelper.GetChromeColor();
        }
    }
}