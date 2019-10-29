using System;
using System.Threading.Tasks;

namespace AutomaticRestore.Common
{
    public class AutomaticCancellationTaskResult<T>
    {

        public TaskStatus TaskStatus { get; private set; }

        public CancellationResons CancellationReson { get; private set; }
        
        public T Result { get; private set; }

        public Exception Error { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        private AutomaticCancellationTaskResult(TaskStatus taskStatus, CancellationResons cancellationReson=CancellationResons.None, T result=default(T), Exception error=null)
        {
            TaskStatus = taskStatus;
            CancellationReson = cancellationReson;
            Result = result;
            Error = error;
        }

        public static AutomaticCancellationTaskResult<T> BuildResult(TaskStatus taskStatus, CancellationResons cancellationReson = CancellationResons.None, T result = default(T), Exception error = null)
        {
            return new AutomaticCancellationTaskResult<T>(taskStatus, cancellationReson, result, error);
        }
    }
}