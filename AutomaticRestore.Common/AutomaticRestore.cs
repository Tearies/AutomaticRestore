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

        public AutomaticRestore()
        {

        }

        private AutomaticRestoreTask LastTask;

        public void CreateRestorePoint()
        {
            ReleaseLastTask();
            LastTask = new AutomaticRestoreTask();
            LastTask.TaskStatuesChanged += LastTask_TaskStatuesChanged;
        }

        public void Dispose()
        {
            ReleaseLastTask();
        }

        private void ReleaseLastTask()
        {
            if (LastTask != null)
            {
                if (LastTask.IsAlive)
                {
                    LastTask.Cancel();
                }

                LastTask.TaskStatuesChanged -= LastTask_TaskStatuesChanged;
                LastTask.Dispose();
            }
        }

        private void LastTask_TaskStatuesChanged(object sender, TaskStatuesChangedEventArgs e)
        {
            Console.WriteLine(e.Status + ":" + e.Result);
        }
    }
}
