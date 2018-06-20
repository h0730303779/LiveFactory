using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common.NetMQ
{
    //public interface ILiveChannelManager
    //{
    //    List<LiveChannelManagerDto> Data { get; set; }
    //    List<PullInfoManagerDto> PullData { get; set; }

    //}

    public static class LiveChannelManager  //: ILiveChannelManager
    {
        public static List<LiveChannelManagerDto> Data { get; set; } = new List<LiveChannelManagerDto>();

        public static List<PullInfoManagerDto> PullData { get; set; } = new List<PullInfoManagerDto>();
    }
}
