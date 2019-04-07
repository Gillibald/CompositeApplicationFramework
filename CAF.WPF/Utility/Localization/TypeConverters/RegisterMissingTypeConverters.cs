#region Usings

using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.TypeConverters
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     Register missing type converters here.
    /// </summary>
    public static class RegisterMissingTypeConverters
    {
        /// <summary>
        ///     A flag indication if the registration was successful.
        /// </summary>
        private static bool registered = false;

        /// <summary>
        ///     Registers the missing type converters.
        /// </summary>
        public static void Register()
        {
            if (registered)
            {
                return;
            }

            TypeDescriptor.AddAttributes(
                typeof (BitmapSource),
                new Attribute[] {new TypeConverterAttribute(typeof (BitmapSourceTypeConverter))});

            registered = true;
        }
    }
}