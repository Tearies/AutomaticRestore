using System;
using System.Threading.Tasks;

namespace AutomaticRestore.Common
{
    public class TaskStatuesChangedEventArgs : EventArgs
    {
        public TaskStatus Status { get; private set; }
        public object Result { get; private set; }
        public Exception Exception { get; private set; }
        public CancellationResons CancellationResons { get; private set; }

        public TaskStatuesChangedEventArgs(TaskStatus status, object result, CancellationResons cancellationResons = CancellationResons.None, Exception exception = null)
        {
            Status = status;
            Result = result;
            Exception = exception;
            CancellationResons = cancellationResons;
        }
    }
}