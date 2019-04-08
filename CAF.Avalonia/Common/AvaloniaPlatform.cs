using Avalonia.Controls;
using CompositeApplicationFramework.Interfaces;

namespace CompositeApplicationFramework.Common
{
    public class AvaloniaPlatform : IPlatform
    {
        public void SetBindingContext(object view, IViewModel context)
        {
            if (view is Control element)
            {
                element.DataContext = context;
            }
        }

        public void SetVisibilityState(object view, bool? visibilityState)
        {
            if (!(view is Control element)) return;

            switch (visibilityState)
            {
                case true:
                    {
                        element.IsVisible = true;
                        break;
                    }
                case false:
                case null:
                    {
                        element.IsVisible = false;
                        break;
                    }
            }
        }
    }
}