#region Usings

using System;
using System.Threading;

#endregion

namespace CompositeApplicationFramework.Utility.EasyLock
{
    public abstract class BaseLock : IDisposable
    {
        protected readonly ReaderWriterLockSlim Lock;

        protected BaseLock(ReaderWriterLockSlim lockSlim)
        {
            Lock = lockSlim;
        }

        public abstract void Dispose();
    }
}