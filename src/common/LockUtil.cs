namespace Microsoft.Hpc
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    //
    // NOTE: All lock-scope objects are not-reentrant by design.  Each lock scope
    // objects must be associated with a single thread.
    //

    abstract class LockScope : IDisposable
    {
#if DEBUG
        int _threadId;
#endif

        protected LockScope()
        {
#if DEBUG
            _threadId = Thread.CurrentThread.ManagedThreadId;
#endif
        }

#if DEBUG
        protected void VerifyThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != _threadId)
            {
                Trace.TraceWarning("VerifyThread thread id is changed for dispose: {0} from created: {1}", Thread.CurrentThread.ManagedThreadId, _threadId);
                throw new InvalidOperationException("The current thread does not own the lock scope object");
            }
        }
#endif

        public void Dispose()
        {
#if DEBUG
            VerifyThread();
#endif
            ExitLock();
        }

        protected abstract void ExitLock();
    }

    abstract class ReaderWriterLockScope : LockScope
    {
        protected ReaderWriterLockSlim _lock;

        protected ReaderWriterLockScope(ReaderWriterLockSlim rwLock) : base()
        {
            if (rwLock == null)
            {
                throw new ArgumentNullException("rwLock");
            }
            _lock = rwLock;
        }
    }

    class ReaderLockScope : ReaderWriterLockScope
    {
        public ReaderLockScope(ReaderWriterLockSlim rwLock)
            : base(rwLock)
        {
            _lock.EnterReadLock();
        }
        protected override void ExitLock()
        {
            _lock.ExitReadLock();
        }
    }

    class WriterLockScope : ReaderWriterLockScope
    {
        public WriterLockScope(ReaderWriterLockSlim rwLock)
            : base(rwLock)
        {
            _lock.EnterWriteLock();
        }
        protected override void ExitLock()
        {
            _lock.ExitWriteLock();
        }
    }

    class UpgradeableLockScope : ReaderWriterLockScope
    {
        public UpgradeableLockScope(ReaderWriterLockSlim rwLock)
            : base(rwLock)
        {
            _lock.EnterUpgradeableReadLock();
        }
        protected override void ExitLock()
        {
            _lock.ExitUpgradeableReadLock();
        }
    }
}