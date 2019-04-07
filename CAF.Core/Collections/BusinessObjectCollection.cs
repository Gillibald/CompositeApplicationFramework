#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class BusinessObjectCollection<TItem> : ObservableCollection<TItem>, IBusinessObject
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

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessObjectCollection{TItem}"/> class.
        /// </summary>
        public BusinessObjectCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessObjectCollection{TItem}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public BusinessObjectCollection(IEnumerable<TItem> items)
        {
            SuppressStateEvents = true;

            foreach (var child in items)
            {
                Add(child);
                child.SetParent(this);
            }

            SuppressStateEvents = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessObjectCollection{TItem}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public BusinessObjectCollection(IParent parent)
        {
            SetParent(parent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessObjectCollection{TItem}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="parent">The parent.</param>
        public BusinessObjectCollection(IEnumerable<TItem> items, IParent parent)
            : this(items)
        {
            SetParent(parent);
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Is executed when the child's state has changed.
        /// </summary>
        /// <param name="child">The child.</param>
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

        /// <summary>
        /// Gets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in edit mode; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether [suppress state events].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [suppress state events]; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        protected bool SuppressStateEvents { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance is self dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is self dirty; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is savable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is savable; otherwise, <c>false</c>.
        /// </value>
        [IgnoreState]
        public bool IsSavable => IsDirty && IsValid;

        /// <summary>
        /// Begins the edit.
        /// </summary>
        public virtual void BeginEdit()
        {
            IsInEditMode = true;
        }

        /// <summary>
        /// Cancels the edit.
        /// </summary>
        public virtual void CancelEdit()
        {
            IsInEditMode = false;
        }

        /// <summary>
        /// Ends the edit.
        /// </summary>
        public virtual void EndEdit()
        {
            IsInEditMode = false;
        }

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified property name.
        /// </summary>
        /// <value>
        /// The <see cref="System.String" />.
        /// </value>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public virtual string this[string propertyName]
        {
            get
            {
                ValidateProperty(propertyName, ref _error);
                return _error;
            }
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public virtual string Error => _error;

        void IParent.ChildStateHasChanged([NotNull] IBusinessObject child)
        {
            if (SuppressStateEvents)
            {
                return;
            }

            ChildStateHasChanged(child);
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
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

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void SetParent(IParent parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Marks the object dirty.
        /// </summary>
        protected void MarkDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Marks the object self dirty.
        /// </summary>
        protected void MarkSelfDirty()
        {
            IsSelfDirty = true;
        }

        /// <summary>
        /// Marks the object clean.
        /// </summary>
        protected void MarkClean()
        {
            IsDirty = false;
        }

        /// <summary>
        /// Marks the object self clean.
        /// </summary>
        protected void MarkSelfClean()
        {
            IsSelfDirty = false;
        }

        /// <summary>
        /// Marks as new.
        /// </summary>
        protected void MarkAsNew()
        {
            IsNew = true;
            IsSelfDirty = true;
        }

        /// <summary>
        /// Marks as old.
        /// </summary>
        protected void MarkAsOld()
        {
            IsNew = false;
            IsSelfDirty = false;
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="error">The error.</param>
        protected virtual void ValidateProperty(string propertyName, ref string error)
        {
        }

        /// <summary>
        /// Raises the state changed.
        /// </summary>
        /// <param name="pPropertyName">Name of the p property.</param>
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

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="pPropertyName">Name of the p property.</param>
        protected virtual void RaisePropertyChanged(string pPropertyName)
        {
            RaiseStateChanged(pPropertyName);

            OnPropertyChanged(new PropertyChangedEventArgs(pPropertyName));
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pPropertyExpression">The p property expression.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> pPropertyExpression)
        {
            var lPropertyName = ExtractPropertyName(pPropertyExpression);
            RaisePropertyChanged(lPropertyName);
        }

        /// <summary>
        /// Extracts the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns></returns>
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
        ~BusinessObjectCollection()
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