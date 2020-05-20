using System;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace WCFDistributedTracing.WCF
{    
    public sealed class DistributedOperationContextScope : IDisposable
    {
        bool _inflight = false;
        bool _disposed;
        OperationContext _thisContext = null;
        OperationContext _originalContext = null;

        public DistributedOperationContextScope(IContextChannel channel) :
            this(new OperationContext(channel))
        {
        }

        public DistributedOperationContextScope(OperationContext context)
        {
            _originalContext = OperationContext.Current;
            OperationContext.Current = _thisContext = context;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_inflight || OperationContext.Current != _thisContext)
                    throw new InvalidOperationException();
                _disposed = true;
                OperationContext.Current = _originalContext;
                _thisContext = null;
                _originalContext = null;
            }
        }

        internal void BeforeAwait()
        {
            if (_inflight)
                return;
            _inflight = true;
            // restore the _originalContext as we might not come back to this thread
            OperationContext.Current = _originalContext;
        }

        internal void AfterAwait()
        {
            if (!_inflight)
                throw new InvalidOperationException();
            _inflight = false;
            // ignore the current context, restore _thisContext
            OperationContext.Current = _thisContext;
        }
    }

    // ContinueOnScope extension
    public static class TaskExt
    {
        public static SimpleAwaiter ContinueOnScope(this Task @this, DistributedOperationContextScope scope)
        {
            return new SimpleAwaiter(@this, scope.BeforeAwait, scope.AfterAwait);
        }

        public static SimpleAwaiter<TResult> ContinueOnScope<TResult>(this Task<TResult> @this, DistributedOperationContextScope scope)
        {
            return new SimpleAwaiter<TResult>(@this, scope.BeforeAwait, scope.AfterAwait);
        }

        public class SimpleAwaiter<TResult> : SimpleAwaiter
        {
            public SimpleAwaiter(Task<TResult> task, Action beforeAwait, Action afterAwait) : 
                base(task, beforeAwait, afterAwait)
            {
            }

            public new SimpleAwaiter<TResult> GetAwaiter()
            {
                return this;
            }

            public new TResult GetResult()
            {
                var task = _task as Task<TResult>;
                return task.Result;
            }
        }

        // awaiter
        public class SimpleAwaiter : INotifyCompletion
        {
            protected Task _task;

            readonly Action _beforeAwait;
            readonly Action _afterAwait;

            public SimpleAwaiter(Task task, Action beforeAwait, Action afterAwait)
            {
                _task = task;
                _beforeAwait = beforeAwait;
                _afterAwait = afterAwait;
            }

            public SimpleAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted
            {
                get
                {
                    // don't do anything if the task completed synchronously
                    // (we're on the same thread)
                    if (_task.IsCompleted)
                        return true;
                    _beforeAwait();
                    return false;
                }

            }

            // INotifyCompletion
            public void OnCompleted(Action continuation)
            {
                _task.ContinueWith(task =>
                {
                    _afterAwait();
                    continuation();
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                SynchronizationContext.Current != null ?
                    TaskScheduler.FromCurrentSynchronizationContext() :
                    TaskScheduler.Current);
            }

            public void GetResult()
            {
            }
        }
    }
}
