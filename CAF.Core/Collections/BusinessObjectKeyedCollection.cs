#region Usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Properties;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Collections
{
    using System.Threading.Tasks;

    public class BusinessObjectKeyedCollection<TKey, TItem> : ObservableKeyedCollection<TKey, TItem>, IBusinessObject
        where TItem : IBusinessObject
    {
        private string _error;
        private bool _hasErrors;
        private bool _isDirty;
        private bool _isInEditMode;
        private bool _isNew;
        private bool _isSelfDirty;
        private bool _isValid = true;
        private IParent _parent;

        public BusinessObjectKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate)
            : base(getKeyForItemDelegate)
        {
        }

        public BusinessObjectKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate, IParent parent)
            : base(getKeyForItemDelegate)
        {
            SetParent(parent);
        }

        public BusinessObjectKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate, IEnumerable<TItem> items)
            : base(getKeyForItemDelegate)
        {
            SuppressStateEvents = true;

            foreach (var child in items)
            {
                Add(child);
                child.SetParent(this);
            }

            SuppressStateEvents = false;
        }

        public BusinessObjectKeyedCollection(
            Func<TItem, TKey> getKeyForItemDelegate,
            IEnumerable<TItem> items,
            IParent parent)
            : this(getKeyForItemDelegate, items)
        {
            SetParent(parent);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (SuppressStateEvents)
            {
                return;
            }

            MarkSelfDirty();

            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (var child in e.NewItems.Cast<TItem>())
            {
                child.SetParent(this);
            }
        }

        protected virtual void ChildStateHasChanged([NotNull] IBusinessObject child)
        {
            if (child.IsDirty && !IsSelfDirty)
            {
                MarkSelfDirty();
            }
            else
            {
                if (!this.Any(x => x.IsDirty))
                {
                    MarkSelfClean();
                }
                else
                {
                    return;
                }
            }

            Parent?.ChildStateHasChanged(this);
        }

        #region IBusinessObject

        [IgnoreState]
        public bool IsInEditMode
        {
            get => _isInEditMode;
            private set
            {
                _isInEditMode = value;
                RaisePropertyChanged(() => IsInEditMode);
            }
        }

        [IgnoreState]
        public bool HasErrors
        {
            get => _hasErrors;
            private set
            {
                _hasErrors = value;
                RaisePropertyChanged(() => HasErrors);
            }
        }

        protected bool SuppressStateEvents { get; set; }

        [IgnoreState]
        public bool IsDirty
        {
            get => IsSelfDirty || _isDirty;
            private set
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
            private set
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
            private set
            {
                _isNew = value;
                RaisePropertyChanged(() => IsNew);
            }
        }

        [IgnoreState]
        public bool IsValid
        {
            get => _isValid;
            private set
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
            private set
            {
                _parent = value;
                RaisePropertyChanged(() => Parent);
            }
        }

        public void SetParent(IParent parent)
        {
            Parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void MarkCleanAfterSave()
        {
            IsSelfDirty = false;
            IsDirty = false;
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

        protected virtual void RaiseStateChanged(string pPropertyName)
        {
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

        private bool HasIgnoreStateAttribute(string propertyName)
        {
            var property =
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).First(x => x.Name == propertyName);
            return property != null && property.GetCustomAttributes(typeof(IgnoreStateAttribute), false).Any();
        }

        #endregion

        #region INotifyPropertyChanged

        protected virtual void RaisePropertyChanged(string pPropertyName)
        {
            RaiseStateChanged(pPropertyName);

            if (PropertyChanged != null && !SuppressStateEvents)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> pPropertyExpression)
        {
            var lPropertyName = ExtractPropertyName(pPropertyExpression);
            RaisePropertyChanged(lPropertyName);
        }

        protected string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var lMemberExpression = propertyExpression.Body as MemberExpression;
            if (lMemberExpression == null)
            {
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_expression_is_not_a_member_access_expression,
                    nameof(propertyExpression));
            }

            var lProperty = lMemberExpression.Member as PropertyInfo;
            if (lProperty == null)
            {
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_member_access_expression_does_not_access_a_property,
                    nameof(propertyExpression));
            }

            if (lProperty.DeclaringType != null && !lProperty.DeclaringType.IsAssignableFrom(GetType()))
            {
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_referenced_property_belongs_to_a_different_type,
                    nameof(propertyExpression));
            }

            var lGetMethod = lProperty.GetGetMethod(true);

            if (lGetMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_referenced_property_does_not_have_a_get_method,
                    nameof(propertyExpression));
            }

            if (lGetMethod.IsStatic)
            {
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_referenced_property_is_a_static_property,
                    nameof(propertyExpression));
            }

            return lMemberExpression.Member.Name;
        }

        #endregion

        #region IDisposable

        /// <summary>
        ///     Callback method
        /// </summary>
        public Action DisposeHandlerCallBack;

        private bool _isDisposed;

        /// <summary>
        ///     Not virtual - implements interface
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalizer from running
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finalizer - in case Dispose is not executed
        /// </summary>
        ~BusinessObjectKeyedCollection()
        {
            Dispose(false);
        }

        public async Task<object> SaveAsync()
        {
            foreach (var item in Items)
            {
                await item.SaveAsync();
            }
            MarkAsOld();
            return this;
        }

        /// <summary>
        ///     Intended to be overridden - always safe to use
        ///     since it will never be called unless applicable
        /// </summary>
        protected virtual void DisposeHandler()
        {
        }

        /// <summary>
        ///     Dispose method - always called
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing)
        {
            // Recursive calls should not attempt
            // to dispose any diposed objects
            if (_isDisposed)
            {
                return;
            }

            // Set flag
            _isDisposed = true;

            // If properly called by Dispose(), not
            // finalizer, then process diposal
            if (!isDisposing)
            {
                return;
            }
            // Provide hook into dispose 
            DisposeHandlerCallBack?.Invoke();

            DisposeHandler();
        }

        #endregion
    }
}