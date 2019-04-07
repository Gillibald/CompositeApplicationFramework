#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class WrappedLock : WrappedLock<object>
    {
        public WrappedLock(Action actionOnLock, Action actionOnUnlock)
            : base(WrapMaybeNull(actionOnLock), WrapMaybeNull(actionOnUnlock))
        {
        }

        public IDisposable GetLock()
        {
            return GetLock(null);
        }

        private static Action<object> WrapMaybeNull(Action action)
        {
            return (param) => { action?.Invoke(); };
        }
    }

    public class WrappedLock<T>
    {
        private readonly Action<T> _actionOnLock;
        private readonly Action<T> _actionOnUnlock;
        private readonly Stack<Guid> _stack = new Stack<Guid>();

        public WrappedLock(Action<T> actionOnLock, Action<T> actionOnUnlock)
        {
            _actionOnLock = actionOnLock ?? ((param) => { });
            _actionOnUnlock = actionOnUnlock ?? ((param) => { });
        }

        public bool IsLocked => _stack.Count > 0;

        public IDisposable GetLock(T param)
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);
            if (_stack.Count == 0)
            {
                _actionOnLock(param);
            }

            var g = Guid.NewGuid();
            _stack.Push(g);
            return new ActionOnDispose(() => Unlock(g, param));
        }

        private void Unlock(Guid g, T param)
        {
            if (_stack.Count > 0 && _stack.Peek() == g)
            {
                _stack.Pop();

                if (_stack.Count == 0)
                {
                    _actionOnUnlock(param);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Unlock happened in the wrong order or at a weird time or too many times");
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_stack != null);
            Contract.Invariant(_actionOnLock != null);
            Contract.Invariant(_actionOnUnlock != null);
        }
    }
}