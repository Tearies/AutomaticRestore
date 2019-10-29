using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticRestore.Common
{
    public abstract class AutomaticCancellationTask<T> : IDisposable
    {
        private AutomaticCancellationTokenSource cts;
        private TaskCompletionSource<T> tcs;
        private Thread workThread;
        internal event EventHandler<TaskErrorEventArgs<T>> TaskError;
        public bool IsAlive => workThread.IsAlive;
        protected bool IsCancellationRequested => cts.IsCancellationRequested;
        private CancellationResons cancellationResons;
        public void Dispose()
        {
            cts.CancellationRequested -= Cts_CancellationRequested;
            cts.Dispose();
            tcs.Task.Dispose();
            tcs = null;
            workThread = null;
        }
        protected AutomaticCancellationTask(TimeSpan timeoutSpan)
        {
            cancellationResons = CancellationResons.None;
            cts = new AutomaticCancellationTokenSource(timeoutSpan);
            tcs = new TaskCompletionSource<T>();//<object>();
            workThread = new Thread(() =>
            {
                try
                {
                    OnTaskStart();
                    var result = OnTaskDoing();
                    if (!IsCancellationRequested)
                    {
                        tcs.SetResult(result);
                        OnTaskEnd();
                    }
                }
                catch (ThreadAbortException tae)
                {
                    Thread.ResetAbort();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }

            });
            workThread.IsBackground = true;
            cts.CancellationRequested += Cts_CancellationRequested;
            tcs.Task.ContinueWith(p =>
            {
                T result = default(T);
                Exception e = null;
                if (p.IsCanceled)
                {
                    result = default(T);
                }
                else if (p.IsFaulted)
                {
                    e = p.Exception;
                }
                else
                {
                    result = p.Result;
                }

                var taskresult =
                    AutomaticCancellationTaskResult<T>.BuildResult(p.Status, cancellationResons, result, e);
                try
                {
                    OnTaskStatuesChanged(taskresult);
                }
                catch (Exception c)
                {
                    OnTaskError(taskresult, c);
                }

            });
            workThread.Start();
        }

        protected abstract void OnTaskStatuesChanged(AutomaticCancellationTaskResult<T> taskResult);

        /// <summary>
        /// 异步执行任务, 该任务超时后会自动回收,不建议该方法内部执行异步方法
        /// </summary>
        /// <returns></returns>
        protected abstract T OnTaskDoing();
         
        protected abstract void OnTaskStart();

        protected abstract void OnTaskEnd();

        private void Cts_CancellationRequested(object sender, AutomaticCancellationEventArgs e)
        {
            var temptcs = tcs;
            var tempthread = workThread;
            if (temptcs.Task.Status != TaskStatus.Canceled
                && temptcs.Task.Status != TaskStatus.Faulted
                && temptcs.Task.Status != TaskStatus.RanToCompletion
                 )
            {
                cancellationResons = e.CancellationReson;
                tempthread.Abort(cancellationResons);
                tempthread.Join();
                temptcs.SetCanceled();
            }
        }
        public void Cancel()
        {
            cts.Cancel();
        }

        protected virtual void OnTaskError(AutomaticCancellationTaskResult<T> e, Exception innerException)
        {
            TaskError?.Invoke(this, new TaskErrorEventArgs<T>(e, innerException));
        }
    }
}