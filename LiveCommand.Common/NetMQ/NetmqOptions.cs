using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common.NetMQ
{
    public static class NetmqOptions
    {
        /// <summary>
        /// 循环器的毫秒-订阅消息
        /// </summary>
        public const int SubscriberMillisecond = 500;

        /// <summary>
        /// 循环器的毫秒-接受客户端消息循
        /// </summary>
        public const int ResopneMillisecond = 500;

        /// <summary>
        /// 循环器的毫秒-监听发送命令
        /// </summary>
        public const int PolicyMillisecond = 5000;

        /// <summary>
        /// 基于检测重复发命令的 发送间隔秒
        /// </summary>
        public const int PolicySendSeconds = 8;

        /// <summary>
        /// 服务器没有收到推流信息的间隔分钟(判断服务器是否down机)
        /// </summary>
        public const int ServiceErrorMinutes = 5;
        /// <summary>
        /// 服务器异常发发送web远端客户端的间隔通知分钟
        /// </summary>
        public const int ServiceErrorSendClientMinutes = 10;
      
    }
}
