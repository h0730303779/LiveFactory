using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LiveCommand.Common
{
    public enum LiveStateType
    {
        [Description("等待推送")]
        Waiting = 0, //等待推送
        [Description("已开始推送")]
        Started = 1, //已开始推送
        [Description("推送中")]
        Pushing = 2,//推送中
        [Description("异常状态")]
        Error = 3,//异常状态
        [Description("停止中")]
        Stoped = 4,//停止中
        [Description("服务器异常")]
        ServiceError = 10//服务器出现异常(多次未接收到服务器反馈的信息)

    }
}
