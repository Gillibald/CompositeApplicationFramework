using System;
using System.Linq;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Interfaces;

namespace CompositeApplicationFramework.Common
{
    public class AvaloniaResourceLoader : IResourceLoader
    {
        public void LoadResources(Type moduleType)
        {
            var attributes =
                moduleType.GetCustomAttributes(typeof(ModuleResourceAttribute), true).Cast<ModuleResourceAttribute>();

            foreach (var resourceUri in
                attributes.Select(attribute => new Uri(attribute.ResourceLocation, UriKind.RelativeOrAbsolute)))
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceInclude { Source = resourceUri });
            }
        }
    }
}
