#region Usings

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CompositeApplicationFramework.Base;

#endregion

namespace CompositeApplicationFramework.ViewModel
{
    public class EnumViewModel : ViewModelBase, IEnumerable
    {
        private Type _enumType;
        private IEnumerable _values;

        public IEnumerable Values
        {
            get => _values;
            protected set
            {
                _values = value;
                RaisePropertyChanged();
            }
        }

        public EnumViewModel()
        {
            _values = Enumerable.Empty<string>();
        }

        public Type EnumType
        {
            get => _enumType;
            set
            {
                _enumType = value;
                RaisePropertyChanged();
                InitValues();
            }
        }

        private void InitValues()
        {
            Values = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static).Select(x => x.GetValue(EnumType));
        }


        public IEnumerator GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}