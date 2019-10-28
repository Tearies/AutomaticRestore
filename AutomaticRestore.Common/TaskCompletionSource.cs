using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticRestore.Common
{
    public class AutomaticTaskSource<TResult>
    {
        public void TrySetResult(object onTaskDoing)
        {
           
        }

        public void TrySetException(Exception exception)
        {
           
        }
         
        public Task<TResult> Task { get; set; }

        public void TrySetCanceled()
        {
            
        }
    }
}