#region Usings

using System.Windows.Markup;
using System.Windows.Media.Imaging;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Extensions
{
    [MarkupExtensionReturnType(typeof (BitmapSource))]
    public class LocImageExtension : LocExtension
    {
        public LocImageExtension()
        {
        }

        public LocImageExtension(string key)
            : base(key)
        {
        }
    }
}