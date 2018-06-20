using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common.NetMQ.Models
{
    public class PublisherListen : SocketData
    {
        public string Topic { get; set; }
        public ISocketData SendData { get; set; }
        public int SendCount { get; set; }

        /// <summary>
        /// 发送日期
        /// </summary>
        public DateTime SendDate { get; set; }
        /// <summary>
        /// 回复日期
        /// </summary>
        public DateTime? ReplyDate { get; set; }

        /// <summary>
        /// 服务器出错后,发送过消息的记录日期
        /// </summary>
        public DateTime? SendWebClientDate { get; set; }
        /// <summary>
        /// 是否发送成功
        /// </summary>
        public bool IsSend { get; set; }
    }
}
