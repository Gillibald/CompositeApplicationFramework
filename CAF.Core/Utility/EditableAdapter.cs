namespace CompositeApplicationFramework.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using CompositeApplicationFramework.Attributes;
    using CompositeApplicationFramework.Interfaces;
    using Types;

    public class EditableAdapter<T> : IEditableObject,
        ICustomTypeDescriptor, INotifyPropertyChanged
    {
        public EditableAdapter(T target)
        {
            Target = target;
            Children = new ObservableCollection<IChangableObject>();
            Changes = new ObservableCollection<IChangedProperty>();
            BeginEdit();
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
            if (Memento == null)
            {
                Memento = new Memento<T>(Target);
            }
        }

        public void CancelEdit()
        {
            if (Memento == null) return;
            Memento.Restore(Target);
            Memento = null;
        }

        public void EndEdit()
        {
            Memento = null;
        }

        private object GetPropertyValue(PropertyInfo info)
        {
            var parcoll = info.GetIndexParameters();
            //To avoid indexed properties that are not easy to cover
            return parcoll.Length != 0 ? null : info.GetValue(Target, null);
        }

        public void UpdateChanges()
        {
            var lChanges = new List<IChangedProperty>();

            foreach (PropertyDescriptor lProperty in ((ICustomTypeDescriptor)this).GetProperties())
            {             
                var lChangedProperty = new ChangedProperty
                {
                    PropertyName = lProperty.Name,
                    PropertyType = lProperty.PropertyType,
                    OldValue = Memento.StoredProperties.FirstOrDefault(x => x.Key.Name == lProperty.Name).Value,
                    NewValue = GetPropertyValue(Target.GetType().GetProperty(lProperty.Name))
                };

                var lInterfaces = lChangedProperty.PropertyType.GetInterfaces();
                //Type[] lGenericTypes = lChangedProperty.GetType().GetGenericArguments();

                if (lInterfaces.Contains(typeof(IDictionary)) && lChangedProperty.NewValue != null)
                {                    
                    var lNewDict = (IDictionary)lChangedProperty.NewValue;

                    if (lChangedProperty.OldValue == null)
                    {
                        lChanges.AddRange(
                            (from object lKey in lNewDict.Keys
                             select new ChangedProperty { NewValue = lNewDict[lKey], Action = ChangeAction.Added }));
                        continue;
                    }

                    var lOldDict = (IDictionary)lChangedProperty.OldValue;

                    //Removed
                    lChanges.AddRange((from object lKey in lOldDict.Keys
                                       where !lNewDict.Contains(lKey)
                                       select new ChangedProperty { OldValue = lOldDict[lKey], Action = ChangeAction.Removed }));

                    //Added
                    lChanges.AddRange((from object lKey in lNewDict.Keys
                                       where !lOldDict.Contains(lKey)
                                       select new ChangedProperty { NewValue = lNewDict[lKey], Action = ChangeAction.Added }));
                }
                else
                {
                    if (lInterfaces.Contains(typeof(IEnumerable)) && lChangedProperty.NewValue != null)
                    {
                        IList lNewCollection = ((IEnumerable)lChangedProperty.NewValue).Cast<object>().ToList();

                        if (lChangedProperty.OldValue == null)
                        {
                            lChanges.AddRange((from object lItem in lNewCollection
                                               select
                                                   new ChangedProperty
                                                   {
                                                       NewValue = lItem,
                                                       Action = ChangeAction.Added,
                                                       PropertyName = "Item",
                                                       PropertyType = lItem.GetType()
                                                   }));
                            continue;
                        }

                        IList lOldCollection = ((IEnumerable)lChangedProperty.OldValue).Cast<object>().ToList();

                        //Removed

                        foreach (var lItem in lOldCollection)
                        {
                            if (!lNewCollection.Contains(lItem))
                            {
                                lChanges.Add(new ChangedProperty { OldValue = lItem, Action = ChangeAction.Removed });
                            }
                            else
                            {
                                if (lOldCollection.IndexOf(lItem) != lNewCollection.IndexOf(lItem))
                                {
                                }
                            }
                        }

                        //Added

                        lChanges.AddRange((from object lItem in lNewCollection
                                           where !lOldCollection.Contains(lItem)
                                           select new ChangedProperty { NewValue = lItem, Action = ChangeAction.Added }));
                    }
                    else
                    {
                        if (lChangedProperty.OldValue != lChangedProperty.NewValue)
                        {
                            lChanges.Add(lChangedProperty);
                        }
                    }
                }
            }

            Changes.Clear();

            foreach (var changedProperty in lChanges)
            {
                var lProperty = (ChangedProperty)changedProperty;
                Changes.Add(lProperty);
            }
        }

        #endregion

        #region ICustomTypeDescriptor Members

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            IList<PropertyDescriptor> lPropertyDescriptors = new List<PropertyDescriptor>();

            var lReadonlyPropertyInfos =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(
                        p =>
                            p.CanRead && !p.CanWrite &&
                            !p.GetCustomAttributes(typeof(ExcludePropertyAttribute), false).Any());

            var lWritablePropertyInfos =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(
                        p =>
                            p.CanRead && p.CanWrite &&
                            !p.GetCustomAttributes(typeof(ExcludePropertyAttribute), false).Any());

            foreach (var lProperty in lReadonlyPropertyInfos)
            {
                var lPropertyCopy = lProperty;

                var lPropertyDescriptor = PropertyDescriptorFactory.CreatePropertyDescriptor(
                    lProperty.Name,
                    typeof(T),
                    lProperty.PropertyType,
                    component => lPropertyCopy.GetValue(((EditableAdapter<T>)component).Target, null));

                lPropertyDescriptors.Add(lPropertyDescriptor);
            }

            foreach (var lProperty in lWritablePropertyInfos)
            {
                var lPropertyCopy = lProperty;

                var lPropertyDescriptor = PropertyDescriptorFactory.CreatePropertyDescriptor(
                    lProperty.Name,
                    typeof(T),
                    lProperty.PropertyType,
                    component => lPropertyCopy.GetValue(
                        ((EditableAdapter<T>)component).Target, null),
                    (component, value) => lPropertyCopy.SetValue(
                        ((EditableAdapter<T>)component).Target, value, null));

                lPropertyDescriptors.Add(lPropertyDescriptor);
            }

            return new PropertyDescriptorCollection(lPropertyDescriptors.ToArray());
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            throw new NotImplementedException();
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            throw new NotImplementedException();
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            throw new NotImplementedException();
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            throw new NotImplementedException();
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptorCollection
            ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        #endregion

        public bool HasChanges
        {
            get
            {
                if (Changes != null)
                {
                    return Changes.Count > 0;
                }
                return false;
            }
        }

        public ObservableCollection<IChangableObject> Children { get; private set; }

        public ObservableCollection<IChangedProperty> Changes { get; private set; }

        /// <summary>
        ///     The wrapped object.
        /// </summary>
        public T Target { get; set; }

        public Memento<T> Memento { get; set; }

        private void NotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #region INotifyPropertyChanged Members

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                if (Target is INotifyPropertyChanged)
                {
                    PropertyChanged += value;
                    ((INotifyPropertyChanged)Target).PropertyChanged += NotifyPropertyChanged;
                }
            }

            remove
            {
                if (Target is INotifyPropertyChanged)
                {
                    PropertyChanged -= value;
                    ((INotifyPropertyChanged)Target).PropertyChanged -= NotifyPropertyChanged;
                }
            }
        }

        private event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}