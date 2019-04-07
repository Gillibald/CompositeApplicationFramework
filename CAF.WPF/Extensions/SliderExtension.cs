#region Usings

using System.Windows.Controls;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     Slider extension
    /// </summary>
    public static class SliderExtension
    {
        /// <summary>
        ///     Set slider value (private scope on view)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Slider SetSliderValue<T>(this T view, string name, double value) where T : UserControl
        {
            var slider = view.FindName(name) as Slider;
            if (slider == null) return null;
            slider.Value = value;
            return slider;
        }
    }
}