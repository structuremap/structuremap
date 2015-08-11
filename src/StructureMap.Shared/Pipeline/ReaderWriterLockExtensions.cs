using System;
using System.Threading;

namespace StructureMap.Pipeline
{
    public static class ReaderWriterLockExtensions
    {
        public static void Write(this ReaderWriterLockSlim rwLock, Action action)
        {
            rwLock.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static T Read<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            rwLock.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static void MaybeWrite(this ReaderWriterLockSlim theLock, Action action)
        {
            try
            {
                theLock.EnterUpgradeableReadLock();
                action();
            }
            finally
            {
                theLock.ExitUpgradeableReadLock();
            }
        }

        public static T MaybeWrite<T>(this ReaderWriterLockSlim theLock, Func<T> answer, Func<bool> missingTest,
            Action write)
        {
            try
            {
                theLock.EnterUpgradeableReadLock();
                if (missingTest())
                {
                    theLock.Write(() => {
                        if (missingTest())
                        {
                            write();
                        }
                    });
                }

                return answer();
            }
            finally
            {
                if (theLock.IsReadLockHeld)
                {
                    theLock.ExitUpgradeableReadLock();
                }
            }
        }
    }
}