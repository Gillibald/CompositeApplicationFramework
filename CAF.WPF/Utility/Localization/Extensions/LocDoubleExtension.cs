#region Usings

using System.Windows.Markup;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Extensions
{
    [MarkupExtensionReturnType(typeof (double))]
    public class LocDoubleExtension : LocExtension
    {
        public LocDoubleExtension()
        {
        }

        public LocDoubleExtension(string key)
            : base(key)
        {
        }
    }
}