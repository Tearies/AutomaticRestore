using System;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace AutomaticRestore.Common
{
    public sealed class AutomaticRestoreTask : AutomaticCancellationTask
    {
        /// <summary>
        /// 异步执行任务, 该任务超时后会自动回收,不建议该方法内部执行异步方法
        /// </summary>
        /// <returns></returns>
        protected override object OnTaskDoing()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                while (!base.IsCancellationRequested)
                {
                    Application.Current.MainWindow.Title = DateTime.Now.ToBinary().ToString();
                }
            });
            return 0;
        }
    }
}