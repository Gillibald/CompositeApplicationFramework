#region Usings

using System.Threading;

#endregion

namespace CompositeApplicationFramework.Utility.EasyLock
{
    public class WriteLock : BaseLock
    {
        public WriteLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetWriteLock(Lock);
        }

        public override void Dispose()
        {
            Locks.ReleaseWriteLock(Lock);
        }
    }
}