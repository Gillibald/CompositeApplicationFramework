#region Usings

using System.ComponentModel;
using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Model
{
    public class ReportEntry : IReportEntry, INotifyPropertyChanged
    {
        private object _assignedObject;
        private string _errorMessage;
        private string _header;
        private string _tooltip;

        public ReportEntry(object pAssignedObject, string pPropertyName)
        {
            AssignedObject = pAssignedObject;
            PropertyName = pPropertyName;
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        public string PropertyName { get; internal set; }

        public object AssignedObject
        {
            get => _assignedObject;
            internal set
            {
                _assignedObject = value;
                RaisePropertyChanged("AssignedObject");
            }
        }

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                RaisePropertyChanged("Header");
            }
        }

        public string Tooltip
        {
            get => _tooltip;
            set
            {
                _tooltip = value;
                RaisePropertyChanged("Tooltip");
            }
        }

        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}