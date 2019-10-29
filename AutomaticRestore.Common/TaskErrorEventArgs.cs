using System;
using System.Threading.Tasks;

namespace AutomaticRestore.Common
{
    public class TaskErrorEventArgs<T> : EventArgs
    {
        public Exception HandeResultError { get; private set; }
        public AutomaticCancellationTaskResult<T> TaskResult { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public TaskErrorEventArgs(AutomaticCancellationTaskResult<T> taskResult, Exception innerException)
        {
            TaskResult = taskResult;
            HandeResultError = innerException;
        }
    }
}