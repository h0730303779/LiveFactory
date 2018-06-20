using LiveCommand.Common.NetMQ;
using LiveCommand.Common.NetMQ.Models;
using System;

namespace LiveCommand.Common
{
    /// <summary>
    /// 命令对象
    /// </summary>
    public class CommandDto: SocketData
    {
        /// <summary>
        /// 工厂code
        /// </summary>
        public string FactoryCode { get; set; }
        /// <summary>
        /// 操作指令
        /// </summary>
        public CommandTypeEnum CommandType { get; set; }
        /// <summary>
        /// 频道id
        /// </summary>
        public Guid ChanneId { get; set; }
        /// <summary>
        /// 拉流地址
        /// </summary>
        public string PullAddress { get; set; }
        /// <summary>
        /// 推流地址
        /// </summary>
        public string PushAddress { get; set; }
    }
}
