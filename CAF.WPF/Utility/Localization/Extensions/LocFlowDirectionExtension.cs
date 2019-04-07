#region Usings

using System.Windows;
using System.Windows.Markup;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Extensions
{
    [MarkupExtensionReturnType(typeof (FlowDirection))]
    public class LocFlowDirectionExtension : LocExtension
    {
        public LocFlowDirectionExtension()
        {
        }

        public LocFlowDirectionExtension(string key)
            : base(key)
        {
        }
    }
}