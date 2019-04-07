#region Usings

using System.Threading;

#endregion

namespace CompositeApplicationFramework.Utility.EasyLock
{
    public static class Locks
    {
        public static void GetReadLock(ReaderWriterLockSlim locks)
        {
            var lockAcquired = false;
            while (!lockAcquired)
            {
                lockAcquired = locks.TryEnterUpgradeableReadLock(1);
            }
        }

        public static void GetReadOnlyLock(ReaderWriterLockSlim locks)
        {
            var lockAcquired = false;
            while (!lockAcquired)
            {
                lockAcquired = locks.TryEnterReadLock(1);
            }
        }

        public static void GetWriteLock(ReaderWriterLockSlim locks)
        {
            var lockAcquired = false;
            while (!lockAcquired)
            {
                lockAcquired = locks.TryEnterWriteLock(1);
            }
        }

        public static void ReleaseReadOnlyLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsReadLockHeld)
            {
                locks.ExitReadLock();
            }
        }

        public static void ReleaseReadLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsUpgradeableReadLockHeld)
            {
                locks.ExitUpgradeableReadLock();
            }
        }

        public static void ReleaseWriteLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsWriteLockHeld)
            {
                locks.ExitWriteLock();
            }
        }

        public static void ReleaseLock(ReaderWriterLockSlim locks)
        {
            ReleaseWriteLock(locks);
            ReleaseReadLock(locks);
            ReleaseReadOnlyLock(locks);
        }

        public static ReaderWriterLockSlim GetLockInstance(
            LockRecursionPolicy recursionPolicy = LockRecursionPolicy.SupportsRecursion)
        {
            return new ReaderWriterLockSlim(recursionPolicy);
        }
    }
}