#region Usings

using System.Windows;
using System.Windows.Markup;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Extensions
{
    #region Dependencies

    

    #endregion

#pragma warning disable 1591

    [MarkupExtensionReturnType(typeof (Thickness))]
    public class LocThicknessExtension : LocExtension
    {
        public LocThicknessExtension()
        {
        }

        public LocThicknessExtension(string key)
            : base(key)
        {
        }
    }
#pragma warning restore 1591
}