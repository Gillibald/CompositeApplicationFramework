﻿#region Usings

using System;
using System.ComponentModel;
using System.Globalization;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.TypeConverters
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     A converter for enum types.
    /// </summary>
    public class EnumConverter : TypeConverter
    {
        private readonly Type targetType;

        /// <summary>
        ///     Create a new <see cref="EnumConverter" />
        /// </summary>
        /// <param name="targetType">The target enum type.</param>
        public EnumConverter(Type targetType)
        {
            this.targetType = targetType;
        }

        /// <summary>
        ///     Returns whether this converter can convert an object of the given type to the type of this converter, using the
        ///     specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        /// <summary>
        ///     Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>An <see cref="Object" /> that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return Enum.Parse(targetType, (string) value, true);
            }
            else
            {
                return null;
            }
        }
    }
}