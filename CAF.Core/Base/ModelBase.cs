#region Usings

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using CompositeApplicationFramework.Extensions;

#endregion

namespace CompositeApplicationFramework.Base
{
    /// <summary>
    ///     Entity base
    /// </summary>
    public abstract class ModelBase : DisposableBase, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the p property.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pPropertyExpression">The p property expression.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> pPropertyExpression)
        {
            var propertyName = pPropertyExpression.GetPropertyName();
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}