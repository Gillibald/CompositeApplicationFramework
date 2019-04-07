#region Usings

using System;
using System.ComponentModel;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IBusinessObject : IEditableObject,
        IDataErrorInfo,
        IDisposable,
        INotifyPropertyChanged,
        IParent,
        ITrackStatus,
        ISavable,
        IEntity
    {
    }
}