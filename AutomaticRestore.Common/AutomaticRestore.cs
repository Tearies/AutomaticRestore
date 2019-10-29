using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace AutomaticRestore.Common
{
    public class AutomaticRestore : IDisposable
    {
        public static readonly AutomaticRestore Default = new Lazy<AutomaticRestore>(() => new AutomaticRestore()).Value;

        private AutomaticRestore()
        {
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
        public void StartAutomaticRestore(TimeSpan intervalTime)
        {
            restoreClock.ConfigTimer(Timeout.InfiniteTimeSpan,intervalTime);
            restoreClock.ConfigTimer(intervalTime, intervalTime);
        }
         
        public void StopAutomaticRestore()
        {
            restoreClock.ConfigTimer(Timeout.InfiniteTimeSpan,Timeout.InfiniteTimeSpan);
        }

        private AutomaticRestoreTask _lastTask;

        private void CreateRestorePoint()
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
