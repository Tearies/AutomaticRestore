using System;
using System.Threading;

namespace AutomaticRestore.Common
{
    public class AutomaticCancellationTokenSource : IDisposable
    {
        private CancellationTokenSource cts;
        public event EventHandler<AutomaticCancellationEventArgs> CancellationRequested;
        private CancellationResons cancellationResons;
        public AutomaticCancellationTokenSource(TimeSpan delay)
        {
            cancellationResons = CancellationResons.TimeOut;
            cts = new CancellationTokenSource(delay);
            cts.Token.Register(OnCancellationRequested);
        }
        protected virtual void OnCancellationRequested()
        {
            CancellationRequested?.Invoke(null, new AutomaticCancellationEventArgs(cancellationResons));
        }

        public void Cancel()
        {
            cancellationResons = CancellationResons.CallCancle;
            cts.Cancel();
        }

        public bool IsCancellationRequested => cts.IsCancellationRequested;

        public void Dispose()
        {
            cts?.Dispose();
        }
    }
}