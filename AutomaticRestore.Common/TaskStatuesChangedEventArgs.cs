using System;
using System.Threading.Tasks;

namespace AutomaticRestore.Common
{
    public class TaskStatuesChangedEventArgs : EventArgs
    {
        public TaskStatus Status { get; private set; }
        public object Result { get; private set; }
        public TaskStatuesChangedEventArgs(TaskStatus status, object pResult)
        {
            Status = status;
            Result = pResult;
        }
    }
}