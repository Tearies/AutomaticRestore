using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
            _lastTask.TaskError += _lastTask_TaskError;
        }

        private void _lastTask_TaskError(object sender, TaskErrorEventArgs<int> e)
        {
           
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

                _lastTask.TaskError -= _lastTask_TaskError;
                _lastTask.Dispose();
            }
        }

        
    }
}
