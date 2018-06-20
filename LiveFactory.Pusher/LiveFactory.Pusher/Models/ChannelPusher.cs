using LiveCommand.Common;
using System;
using System.Threading;

namespace LiveFactory.Pusher
{
    public class ChannelPusher
    {
        public CommandDto CommandDto { get; set; }

        //public LogDto LogDto { get; set; }

        /// <summary>
        /// markup cancel pushing or not
        /// </summary>
        public CancellationTokenSource CTS { get; set; }

        public ChannelPusher()
        {
            //LogDto = new LogDto();
            CTS = new CancellationTokenSource();
        }
    }
}
