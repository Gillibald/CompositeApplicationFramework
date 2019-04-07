#region Usings

using System.Linq;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace CompositeApplicationFramework.Utility
{
    #region Dependencies

    

    #endregion

    public class DynamicTemplateSelector : DataTemplateSelector
    {
        public static readonly DependencyProperty TemplatesProperty = DependencyProperty.RegisterAttached(
            "Templates",
            typeof (TemplateCollection),
            typeof (DynamicTemplateSelector),
            new FrameworkPropertyMetadata(new TemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public static TemplateCollection GetTemplates(DependencyObject pContainer)
        {
            return (TemplateCollection) pContainer.GetValue(TemplatesProperty);
        }

        public static void SetTemplates(DependencyObject pObj, TemplateCollection pCollection)
        {
            pObj.SetValue(TemplatesProperty, pCollection);
        }

        public override DataTemplate SelectTemplate(object pItem, DependencyObject pContainer)
        {
            if (pContainer == null)
            {
                return base.SelectTemplate(pItem, null);
            }

            var lTemplates = GetTemplates(pContainer);
            if (lTemplates == null || lTemplates.Count == 0)
            {
                return base.SelectTemplate(pItem, pContainer);
            }

            foreach (var lTemplate in lTemplates.Where(lTemplate => lTemplate.IsMatch(pItem)))
            {
                return lTemplate.DataTemplate;
            }

            return base.SelectTemplate(pItem, pContainer);
        }
    }
}