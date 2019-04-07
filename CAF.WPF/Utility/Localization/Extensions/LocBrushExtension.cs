#region Usings

using System.Windows.Markup;
using System.Windows.Media;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Extensions
{
    [MarkupExtensionReturnType(typeof (Brush))]
    public class LocBrushExtension : LocExtension
    {
        public LocBrushExtension()
        {
        }

        public LocBrushExtension(string key)
            : base(key)
        {
        }
    }
}