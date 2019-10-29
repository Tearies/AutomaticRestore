using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace AutomaticRestore.Common
{
    public class AutomaticRestore<T> : IDisposable where T : AutomaticRestoreConfiguration
    {
        public static readonly AutomaticRestore<T> Default = new Lazy<AutomaticRestore<T>>(() => new AutomaticRestore<T>()).Value;

        private T configuration;

        private AutomaticRestore()
        {
            configuration = Activator.CreateInstance<T>();
            restoreClock = new AutomaticRestoreClock();
            restoreClock.ClockTicked += RestoreClock_ClockTicked;
        }

        private void RestoreClock_ClockTicked(object sender, EventArgs e)
        {
            CreateRestorePoint();
        }

        private AutomaticRestoreClock restoreClock;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intervalTime"></param>
        public void StartAutomaticRestore()
        {
            if (configuration.IsEnabled)
            {
                //暂停
                restoreClock.ConfigTimer(Timeout.InfiniteTimeSpan, configuration.IntervalTime);
                //开始
                restoreClock.ConfigTimer(configuration.IntervalTime, configuration.IntervalTime);
            }

        }

        public void StopAutomaticRestore()
        {
            restoreClock.ConfigTimer(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private AutomaticRestoreTask _lastTask;

        private void CreateRestorePoint()
        {
            ReleaseLastTask();
            _lastTask = new AutomaticRestoreTask(configuration.TaskTimeOut);
            _lastTask.TaskError += _lastTask_TaskError;
        }

        private void _lastTask_TaskError(object sender, TaskErrorEventArgs<int> e)
        {

        }

        public void Dispose()
        {
            restoreClock.ClockTicked -= RestoreClock_ClockTicked;
            restoreClock.Dispose();
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
