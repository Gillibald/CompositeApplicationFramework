#region Usings

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Collections;
using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Model
{
    public class ReportItem<T> : ModelBase, IReportItem
        where T : INotifyPropertyChanged
    {
        private readonly ObservableKeyedCollection<object, IReportEntry> _entries =
            new ObservableKeyedCollection<object, IReportEntry>(x => x.AssignedObject);

        private T _assignedObject;
        private string _category;
        private string _header;
        private string _tooltip;

        public ReportItem(T pAssignedObject, string pCategory)
        {
            AssignedObject = pAssignedObject;
            Category = pCategory;
        }

        public T AssignedObject
        {
            get => _assignedObject;
            internal set
            {
                _assignedObject = value;
                RaisePropertyChanged(() => AssignedObject);
            }
        }

        public IEnumerable<IReportEntry> Entries => _entries;

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                RaisePropertyChanged(() => Header);
            }
        }

        public string Tooltip
        {
            get => _tooltip;
            set
            {
                _tooltip = value;
                RaisePropertyChanged(() => Tooltip);
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                RaisePropertyChanged(() => Category);
            }
        }

        public void AddEntry(IReportEntry pEntry)
        {
            if (!Entries.Contains(pEntry.AssignedObject))
            {
                _entries.Add(pEntry);
            }
        }

        public void RemoveEntry(IReportEntry pEntry)
        {
            if (Entries.Contains(pEntry.AssignedObject))
            {
                _entries.Remove(pEntry);
            }
        }
    }
}