using NetMQ.Sockets;
using System;

namespace LiveCommand.Common.NetMQ
{

    public static class NetMQFactory
    {
        /// <summary>
        /// 发布Socket
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static PublisherSocket CreatePublisherSocker(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            var socket = new PublisherSocket();
            socket.Bind(url);
            return socket;
        }

        public static SubscriberSocket CreateSubscriberSocket(string url, string topic)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentNullException("topic");

            var socket = new SubscriberSocket();
            socket.Connect(url);
            socket.Subscribe(topic);
            return socket;
        }

        public static RequestSocket CreateRequestSocket(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            if (!url.StartsWith(">"))
                throw new ArgumentNullException("url 地址必须以 \">\" 开头。");

            return new RequestSocket(url);
        }

        public static ResponseSocket CreateResponseSocket(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            if (!url.StartsWith("@"))
                throw new ArgumentNullException("url 地址必须以 \"@\" 开头。");

            var socket = new ResponseSocket(url);
            return socket;
        }

    }
}
