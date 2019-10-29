using System;
using System.Threading;

namespace AutomaticRestore.Common
{
    public class AutomaticRestoreClock : IDisposable
    {
        private Timer clock;

        public event EventHandler ClockTicked;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AutomaticRestoreClock()
        {
            clock = new Timer(OnClockTicked, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void OnClockTicked(object state)
        {
            OnClockTicked();
        }

        private  void OnClockTicked()
        {
            try
            {
                ClockTicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {

            }
           
        }

        public void ConfigTimer(TimeSpan dueTimer, TimeSpan interval)
        {
            clock.Change((long)dueTimer.TotalMilliseconds, (long)interval.TotalMilliseconds);
        }

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            clock?.Dispose();
        }

        #endregion
    }
}