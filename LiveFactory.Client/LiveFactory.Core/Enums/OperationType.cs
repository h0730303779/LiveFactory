using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace LiveFactory.Core.Enums
{
    public enum OperationType
    {
        [Description("直播命令操作")]
        Command = 1,
        [Description("直播策略操作")]
        Policy = 2,
    }
}
