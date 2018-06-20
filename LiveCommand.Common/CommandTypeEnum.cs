using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace LiveCommand.Common
{
    public enum CommandTypeEnum
    {
        /// <summary>
        /// 切换
        /// </summary>
        Switch=3,

        /// <summary>
        /// 重启
        /// </summary>
        [Description("重启")]
        ReStart =2,
        /// <summary>
        /// 播放
        /// </summary>
        [Description("播放")]
        Start = 1,

        /// <summary>
        /// 暂停
        /// </summary>
        [Description("暂停")]
        Stop = 0,
    }
}
