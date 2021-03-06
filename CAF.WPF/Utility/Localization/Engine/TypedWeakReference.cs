﻿#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.Engine
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     A types version of <see cref="WeakReference" />.
    /// </summary>
    /// <typeparam name="T">The reference type.</typeparam>
    public class TypedWeakReference<T> : WeakReference
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="target">The target.</param>
        public TypedWeakReference(T target)
            : base(target)
        {
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">The track resurrection flag.</param>
        public TypedWeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected TypedWeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public new T Target
        {
            get
            {
                var baseTarget = base.Target;
                if (IsAlive && baseTarget != null)
                {
                    return (T) base.Target;
                }
                return default(T);
            }
            set => base.Target = value;
        }
    }
}