using LiveCommand.Common.NetMQ.Models;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Text;

namespace LiveCommand.Common.NetMQ
{
    public interface INetMQManage
    {

        bool Publish(string topic, ISocketData sendDto);
        string ReceiveSubscriber(string topic);
        T ReceiveSubscriber<T>(string topic) where T : ISocketData;

        T RequestReceive<T>(IRequestData sendDto) where T : IResponseData;
        void ResponseReceive(Func<string, IResponseData> func);

        void Cleanup();

    }

    public class NetMQManage : INetMQManage
    {

        #region url地址
        private readonly string _publisherUrl;
        private readonly string _responseUrl;

        public NetMQManage(string publisherUrl, string responseUrl)
        {
            _publisherUrl = publisherUrl;
            _responseUrl = responseUrl;
        }
        #endregion

        object asyncLock = new object();

        #region 发布对象
        /// <summary>
        /// 发布
        /// </summary>
        private static PublisherSocket _publisherSocket;
        private PublisherSocket PublisherSocket
        {
            get
            {
                if (_publisherSocket == null)
                {
                    lock (asyncLock)
                    {
                        if (_publisherSocket == null)
                        {
                            return _publisherSocket = NetMQFactory.CreatePublisherSocker(_publisherUrl);
                        }
                    }
                }
                return _publisherSocket;
            }

        }
        #endregion

        #region 订阅对象
        static SubscriberSocket SubscriberSocket;
        #endregion

        #region 接受服务对象
        private static ResponseSocket _responseSocket;
        private ResponseSocket ResponseSocket
        {
            get
            {
                if (_responseSocket == null)
                {
                    lock (asyncLock)
                    {
                        if (_responseSocket == null)
                        {
                            return _responseSocket = NetMQFactory.CreateResponseSocket("@" + _responseUrl);
                        }
                    }
                }
                return _responseSocket;
            }
        }
        #endregion

        #region 发送请求对象
        static RequestSocket _requestSocket;
        private RequestSocket RequestSocket
        {
            get
            {
                if (_requestSocket == null)
                {
                    lock (asyncLock)
                    {
                        if (_requestSocket == null)
                        {
                            return _requestSocket = NetMQFactory.CreateRequestSocket(">" + _responseUrl);
                        }
                    }
                }
                return _requestSocket;
            }
        }

        #endregion

        #region 发布消息
        public bool Publish(string topic, ISocketData sendDto)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentNullException(topic);
            }
            try
            {
                var sendVal = JsonConvert.SerializeObject(sendDto);
                sendVal = topic + "/" + sendVal;
                var valBytes = Encoding.UTF8.GetBytes(sendVal);
                // var isSend = PublisherSocket.SendMoreFrame(topic).TrySendFrameBytes(sendVal);
                var isSend = PublisherSocket.TrySendFrameBytes(sendVal);
                if (isSend)
                    LogWrite.Error("[发布消息]消息已广播出去:" + sendVal);
                return isSend;
            }
            catch (Exception e)
            {
                LogWrite.Error(e);
                return false;
            }
        }
        #endregion

        #region 订阅消息
        public string ReceiveSubscriber(string topic)
        {
            try
            {
                if (SubscriberSocket == null)
                    SubscriberSocket = NetMQFactory.CreateSubscriberSocket(_publisherUrl, topic);
                //string messageTopicReceived = SubscriberSocket.ReceiveFrameString();
                var result = SubscriberSocket.ReceiveFrameBytesSTR();
                LogWrite.Error("[订阅消息]收到消息:" + result);
                return result.Substring(topic.Length + 1);
            }
            catch (Exception ex)
            {
                LogWrite.Error(ex);
                return string.Empty;
            }
        }

        public T ReceiveSubscriber<T>(string topic) where T : ISocketData
        {
            try
            {
                var receive = ReceiveSubscriber(topic);
                return JsonConvert.DeserializeObject<T>(receive);
            }
            catch (Exception e)
            {
                LogWrite.Error(e);
                return default(T);
            }
        }
        #endregion

        #region 发送请求
        static object lockObj = new object();
        private string RequestReceive(IRequestData sendDto)
        {
            lock (lockObj)
            {
                try
                {
                    RequestSocket.TrySendFrameBytes(sendDto);
                    var receive = RequestSocket.ReceiveFrameBytesSTR();
                    return receive;
                }
                catch (Exception e)
                {
                    LogWrite.Error(e);
                    return string.Empty;
                }
            }
        }
        public T RequestReceive<T>(IRequestData sendDto) where T : IResponseData
        {
            try
            {
                var str = RequestReceive(sendDto);
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception e)
            {
                LogWrite.Error(e);
                return default(T);
            }
        }

        #endregion

        #region 接收
        public void ResponseReceive(Func<string, IResponseData> ReceiveCallback)
        {
            try
            {
                var receiveStr = ResponseSocket.ReceiveFrameBytesSTR();
                LogWrite.Error("[收消息]收到客户端消息:" + receiveStr);
                var responseInfo = ReceiveCallback(receiveStr);
                ResponseSocket.TrySendFrameBytes(responseInfo);
            }
            catch (Exception e)
            {

                LogWrite.Error(e);
            }

        }
        #endregion
        public void Cleanup()
        {
            NetMQConfig.Cleanup(false);
        }
    }
}
