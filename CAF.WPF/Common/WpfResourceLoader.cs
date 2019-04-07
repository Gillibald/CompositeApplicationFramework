using System;
using System.Linq;
using System.Windows;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Interfaces;

namespace CompositeApplicationFramework.Common
{
    public class WpfResourceLoader : IResourceLoader
    {
        public void LoadResources(Type moduleType)
        {
            var attributes =
                moduleType.GetCustomAttributes(typeof(ModuleResourceAttribute), true).Cast<ModuleResourceAttribute>();

            foreach (var resourceUri in
                attributes.Select(attribute => new Uri(attribute.ResourceLocation, UriKind.RelativeOrAbsolute)))
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = resourceUri });
            }
        }
    }
}
