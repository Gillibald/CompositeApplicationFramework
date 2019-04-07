#region Usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace CompositeApplicationFramework.Converter
{
    #region Dependencies

    

    #endregion

    internal class MaximizeVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ResizeMode) value == ResizeMode.NoResize || (ResizeMode) value == ResizeMode.CanMinimize
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}