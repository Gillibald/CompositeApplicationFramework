#region Usings

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Engine
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     An extended combobox that is enumerating Enum values.
    ///     <para>Use the <see cref="BrowsableAttribute" /> to hide specific entries.</para>
    /// </summary>
    public class EnumComboBox : ComboBox
    {
        /// <summary>
        ///     The Type.
        /// </summary>
        public static DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type",
            typeof (Type),
            typeof (EnumComboBox),
            new PropertyMetadata(TypeChanged));

        /// <summary>
        ///     The backing property for <see cref="EnumComboBox.TypeProperty" />
        /// </summary>
        [Category("Common")]
        public Type Type
        {
            get => (Type) GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ecb = d as EnumComboBox;

            ecb?.SetType(ecb.Type);
        }

        private void SetType(Type type)
        {
            try
            {
                // First we need to get list of all enum fields
                var fields = type.GetFields();

                var items = (from field
                    in fields
                    where !field.IsSpecialName
                    let attr = field.GetCustomAttributes(false).OfType<BrowsableAttribute>().FirstOrDefault()
                    where attr == null || attr.Browsable
                    select field.GetValue(0)).ToList();

                ItemsSource = items;
            }
            catch
            {
                // ignored
            }
        }
    }
}