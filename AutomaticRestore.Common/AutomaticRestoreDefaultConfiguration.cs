using System;

namespace AutomaticRestore.Common
{
    public class AutomaticRestoreDefaultConfiguration : AutomaticRestoreConfiguration
    {
        public AutomaticRestoreDefaultConfiguration()
        {
            IsEnabled = true;
            IntervalTime = TimeSpan.FromSeconds(5);
            TaskTimeOut = TimeSpan.FromSeconds(4);
        }

        #region Overrides of AutomaticRestoreConfiguration

        /// <summary>
        /// 是否开启
        /// </summary>
        public override bool IsEnabled { get; }

        /// <summary>
        /// 间隔时间
        /// </summary>
        public override TimeSpan IntervalTime { get; }

        /// <summary>
        /// 任务超时时间
        /// </summary>
        public override TimeSpan TaskTimeOut { get; }

        #endregion
    }
}