using LiveCommand.Common.NetMQ.Models;
using NetMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LiveCommand.Common.NetMQ
{
    public static class NetMQExtensions
    {
        public static bool TrySendFrameBytes(this IOutgoingSocket socket, string Message)
        {
            var bytes = Encoding.UTF8.GetBytes(Message);
            var result = socket.TrySendFrame(bytes);
            return result;
        }
        public static bool TrySendFrameBytes(this IOutgoingSocket socket, object MessageObj)
        {
            var json = JsonConvert.SerializeObject(MessageObj);
            var result = TrySendFrameBytes(socket, json);
            return result;
        }

        public static string ReceiveFrameBytesSTR(this IReceivingSocket socket)
        {
            var resultBytes = socket.ReceiveFrameBytes();
            if (resultBytes == null || resultBytes.Length == 0) return string.Empty;
            return Encoding.UTF8.GetString(resultBytes);
        }


        public static void AddListens(this List<PublisherListen> listens, string topic, ISocketData sendDto)
        {
            //记录发送对象
            var first = listens.FirstOrDefault(b => b.Id == sendDto.Id);
            if (first == null)
                listens.Add(new PublisherListen() { Id = sendDto.Id, SendCount = 1, SendData = sendDto, SendDate = DateTime.Now, Topic = topic });
            else
            {
                first.SendCount += 1;
                first.SendDate = DateTime.Now;
            }
        }
        public static void ResultListens(this List<PublisherListen> listens, Guid Id)
        {
            /*
             如果状态是未发送
            收到消息后,当前这条命令消息设置成已发发送成功
             */

            var first = listens.FirstOrDefault(b => !b.IsSend && b.Id == Id);
            if (first == null)
                return;
            else
            {
                first.ReplyDate = DateTime.Now;
                first.IsSend = true;
            }
        }
        public static bool IsSendListens(this PublisherListen item)
        {
            /*
             * 如果状态是未发送
             判断对未收到反馈消息的发送命令进行处理再次发送
             发送间隔超过 10s 发送次数>4
             */
            if (item.IsSend) return false;
            if (item.SendCount > 4) return false;
            var seconds = (DateTime.Now - item.SendDate).Seconds;
            if (seconds < NetmqOptions.PolicySendSeconds)
                return false;
            return true;
        }

        public static bool IsListensServiceError(this PublisherListen item)
        {
            /*
             检测服务器是否出现异常
             */

            bool result = false;
            if (checkDate(item.SendDate))
            {
                if (item.ReplyDate != null)
                {
                    if (checkDate(item.SendDate))
                    {
                        result = true;
                    }
                }
                result = true;
            }
            if (result && item.SendWebClientDate != null)
            {
                var minutes = (DateTime.Now - (DateTime)item.SendWebClientDate).Minutes;
                if (minutes < NetmqOptions.ServiceErrorSendClientMinutes)
                    result = false;

            }
            return result;
        }
        private static bool checkDate(DateTime time)
        {
            var minutes = (DateTime.Now - time).Minutes;
            if (minutes > NetmqOptions.ServiceErrorMinutes)
                return true;
            return false;
        }
    }
}
