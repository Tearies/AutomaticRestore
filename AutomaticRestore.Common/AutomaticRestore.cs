using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace AutomaticRestore.Common
{
    public class AutomaticRestore : IDisposable
    {
        public static readonly AutomaticRestore Default = new Lazy<AutomaticRestore>(() => new AutomaticRestore()).Value;
        private AutomaticRestore()
        {

        }

        private AutomaticRestoreTask _lastTask;

        public void CreateRestorePoint()
        {
            ReleaseLastTask();
            _lastTask = new AutomaticRestoreTask();
            _lastTask.TaskStatuesChanged += LastTask_TaskStatuesChanged;
        }

        public void Dispose()
        {
            ReleaseLastTask();
        }

        private void ReleaseLastTask()
        {
            if (_lastTask != null)
            {
                if (_lastTask.IsAlive)
                {
                    _lastTask.Cancel();
                }

                _lastTask.TaskStatuesChanged -= LastTask_TaskStatuesChanged;
                _lastTask.Dispose();
            }
        }

        private void LastTask_TaskStatuesChanged(object sender, TaskStatuesChangedEventArgs e)
        {
            Console.WriteLine(e.Status + ":" + e.Result + "  " + e.CancellationResons + "    " + e.Exception);
        }
    }
}
