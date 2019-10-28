using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticRestore.Common
{
    public abstract class AutomaticCancellationTask : IDisposable
    {
        private AutomaticCancellationTokenSource cts;
        private TaskCompletionSource<object> tcs;
        private Thread workThread;
        public event EventHandler<TaskStatuesChangedEventArgs> TaskStatuesChanged;
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
            tcs = new TaskCompletionSource<object>();//<object>();
            workThread = new Thread(() =>
            {
                try
                {
                    Console.WriteLine("Start");
                    var result = OnTaskDoing();
                    if (!IsCancellationRequested)
                    {
                        tcs.SetResult(result);
                        Console.WriteLine("End");
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
                object result = null;
                Exception e = null;
                if (p.IsCanceled  )
                {
                    result = null;
                }
                else if(p.IsFaulted)
                {
                    e = p.Exception;
                }
                else
                {
                    result = p.Result;
                }
                OnTaskStatuesChanged(p.Status, result, cancellationResons,e);
            });
            workThread.Start();
        }

        private void OnTaskStatuesChanged(TaskStatus pStatus, object pResult, CancellationResons cancellationResons1, Exception exception)
        {
            TaskStatuesChanged?.Invoke(this, new TaskStatuesChangedEventArgs(pStatus, pResult, cancellationResons1, exception));
        }

        /// <summary>
        /// 异步执行任务, 该任务超时后会自动回收,不建议该方法内部执行异步方法
        /// </summary>
        /// <returns></returns>
        protected abstract object OnTaskDoing();

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

        
    }
}