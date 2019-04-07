#region Usings

using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Model;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Base
{
    using System.Threading.Tasks;

    public abstract class BusinessObjectBase<T> : BusinessObjectBase, ISavable<T>
        where T : BusinessObjectBase<T>
    {
        [Browsable(false)]
        public ReportItem<T> Report { get; set; }

        public new virtual Task<T> SaveAsync()
        {
            MarkAsOld();
            return Task.FromResult((T)this);
        }
    }

    public abstract class BusinessObjectBase : ModelBase, IBusinessObject
    {
        private string _error;
        private bool _hasErrors;
        private bool _isDirty;
        private bool _isInEditMode;
        private bool _isNew = true;
        private bool _isSelfDirty;
        private bool _isValid = true;
        private IParent _parent;

        /// <summary>
        /// Gets or sets a value indicating whether [suppress state events].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [suppress state events]; otherwise, <c>false</c>.
        /// </value>
        protected bool SuppressStateEvents { get; set; }

        [IgnoreState]
        [Browsable(false)]
        public bool IsInEditMode
        {
            get => _isInEditMode;
            protected set
            {
                _isInEditMode = value;
                RaisePropertyChanged(() => IsInEditMode);
            }
        }

        [IgnoreState]
        [Browsable(false)]
        public bool HasErrors
        {
            get => _hasErrors;
            protected set
            {
                _hasErrors = value;
                RaisePropertyChanged(() => HasErrors);
            }
        }

        [IgnoreState]
        public bool IsDirty
        {
            get => IsSelfDirty || _isDirty;
            protected set
            {
                _isDirty = value;
                RaisePropertyChanged(() => IsDirty);
                RaisePropertyChanged(() => IsSavable);
            }
        }

        [IgnoreState]
        public bool IsSelfDirty
        {
            get => _isSelfDirty;
            protected set
            {
                _isSelfDirty = value;
                Parent?.ChildStateHasChanged(this);
                RaisePropertyChanged(() => IsSelfDirty);
                RaisePropertyChanged(() => IsSavable);
            }
        }

        [IgnoreState]
        public bool IsNew
        {
            get => _isNew;
            protected set
            {
                _isNew = value;
                RaisePropertyChanged(() => IsNew);
            }
        }

        [IgnoreState]
        public bool IsValid
        {
            get => _isValid;
            protected set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
                RaisePropertyChanged(() => IsSavable);
            }
        }

        [IgnoreState]
        public bool IsSavable => IsDirty && IsValid;

        public virtual void BeginEdit()
        {
            IsInEditMode = true;
        }

        public virtual void CancelEdit()
        {
            IsInEditMode = false;
        }

        public virtual void EndEdit()
        {
            IsInEditMode = false;
        }

        public virtual string this[string propertyName]
        {
            get
            {
                ValidateProperty(propertyName, ref _error);
                return _error;
            }
        }

        [Browsable(false)]
        public virtual string Error => _error;

        void IParent.ChildStateHasChanged([NotNull] IBusinessObject child)
        {
            if (SuppressStateEvents)
            {
                return;
            }

            ChildStateHasChanged(child);
        }

        [IgnoreState]
        public IParent Parent
        {
            get => _parent;
            protected set
            {
                _parent = value;
                RaisePropertyChanged(() => Parent);
            }
        }

        public void SetParent(IParent parent)
        {
            _parent = parent;
        }

        protected void MarkDirty()
        {
            IsDirty = true;
        }

        protected void MarkSelfDirty()
        {
            IsSelfDirty = true;
        }

        protected void MarkClean()
        {
            IsDirty = false;
        }

        protected void MarkSelfClean()
        {
            IsSelfDirty = false;
        }

        protected void MarkAsNew()
        {
            IsNew = true;
            IsSelfDirty = true;
        }

        protected void MarkAsOld()
        {
            IsNew = false;
            IsSelfDirty = false;
        }

        protected virtual void ValidateProperty(string propertyName, ref string error)
        {
        }

        protected virtual void ChildStateHasChanged([NotNull] IBusinessObject child)
        {
            if (child.IsDirty)
            {
                MarkDirty();
            }
            else
            {
                MarkClean();
            }

            Parent?.ChildStateHasChanged(this);
        }

        protected virtual void RaiseStateChanged(string pPropertyName)
        {
            if (string.IsNullOrEmpty(pPropertyName)) return;

            if (SuppressStateEvents)
            {
                return;
            }

            if (HasIgnoreStateAttribute(pPropertyName))
            {
                return;
            }

            MarkSelfDirty();
#if DEBUG
            Debug.WriteLine("BusinessObject: {0} has changed Property: {1}", ToString(), pPropertyName);
#endif
            Parent?.ChildStateHasChanged(this);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (SuppressStateEvents) return;
            RaiseStateChanged(propertyName);
            base.OnPropertyChanged(propertyName);
        }
        //todo:Caching
        /// <summary>
        /// Determines whether [has ignore state attribute] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private bool HasIgnoreStateAttribute(string propertyName)
        {
            var property =
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).First(x => x.Name == propertyName);
            return property != null && property.GetCustomAttributes(typeof(IgnoreStateAttribute), false).Any();
        }

        protected class StateEventsSuppressor : DisposableBase
        {
            private readonly BusinessObjectBase _businessObject;

            public StateEventsSuppressor(BusinessObjectBase businessObject)
            {
                _businessObject = businessObject;
                businessObject.SuppressStateEvents = true;
            }

            protected override void DisposeHandler()
            {
                _businessObject.SuppressStateEvents = false;

                base.DisposeHandler();
            }
        }

        public Task<object> SaveAsync()
        {
            MarkAsOld();
            return Task.FromResult<object>(this);
        }
    }
}