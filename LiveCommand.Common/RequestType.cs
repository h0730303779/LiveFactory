using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common
{
    public enum RequestType
    {
        /// <summary>
        /// 服务日志
        /// </summary>
        ServiceLog,
        /// <summary>
        /// 推流日志
        /// </summary>
        PushLog,
        /// <summary>
        /// 云端推送命令给视频源客户端
        /// </summary>
        Command
    }
}
