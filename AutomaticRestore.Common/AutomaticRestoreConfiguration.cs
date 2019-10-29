using System;

namespace AutomaticRestore.Common
{
    public abstract class AutomaticRestoreConfiguration
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public abstract bool IsEnabled { get;}

        /// <summary>
        /// 间隔时间
        /// </summary>
        public abstract TimeSpan IntervalTime { get; }

        /// <summary>
        /// 任务超时时间
        /// </summary>
        public abstract TimeSpan TaskTimeOut { get; }
    }
}