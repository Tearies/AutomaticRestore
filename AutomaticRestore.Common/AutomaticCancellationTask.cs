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
        public void Dispose()
        {
            cts.CancellationRequested -= Cts_CancellationRequested;
            cts.Dispose();
            tcs.Task.Dispose();
            tcs = null;
            workThread = null;
        }
        public AutomaticCancellationTask()
        {
            cts = new AutomaticCancellationTokenSource(TimeSpan.FromSeconds(10));
            tcs = new TaskCompletionSource<object>();//<object>();
            workThread = new Thread(() =>
            {
                try
                {
                    Console.WriteLine("Start");
                    tcs.TrySetResult(OnTaskDoing());
                    Console.WriteLine("End");
                }
                catch (ThreadAbortException tae)
                {
                    Thread.ResetAbort();
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }

            });
            workThread.IsBackground = true;
            cts.CancellationRequested += Cts_CancellationRequested;
            tcs.Task.ContinueWith(p =>
            {
                object result = null;
                if (p.IsCanceled || p.IsFaulted)
                {
                    result = null;
                }
                else
                {
                    result = p.Result;
                }
                OnTaskStatuesChanged(p.Status, result);
            });
            workThread.Start();
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

                tempthread.Abort(e.CancellationReson);
                tempthread.Join();
                temptcs.TrySetCanceled();
            }
        }
        public void Cancel()
        {
            cts.Cancel();
        }

        protected void OnTaskStatuesChanged(TaskStatus e, object pResult)
        {
            TaskStatuesChanged?.Invoke(this, new TaskStatuesChangedEventArgs(e, pResult));
        }
    }
}