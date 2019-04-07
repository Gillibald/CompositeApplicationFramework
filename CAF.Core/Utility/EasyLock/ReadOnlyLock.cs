#region Usings

using System.Threading;

#endregion

namespace CompositeApplicationFramework.Utility.EasyLock
{
    public class ReadOnlyLock : BaseLock
    {
        public ReadOnlyLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetReadOnlyLock(Lock);
        }

        public override void Dispose()
        {
            Locks.ReleaseReadOnlyLock(Lock);
        }
    }
}