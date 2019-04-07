#region Usings

using System.Threading;

#endregion

namespace CompositeApplicationFramework.Utility.EasyLock
{
    public class ReadLock : BaseLock
    {
        public ReadLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetReadLock(Lock);
        }

        public override void Dispose()
        {
            Locks.ReleaseReadLock(Lock);
        }
    }
}