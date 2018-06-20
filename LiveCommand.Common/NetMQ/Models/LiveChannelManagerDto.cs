using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common.NetMQ
{

    //[AutoMap(typeof(JFJT LiveChannel))]
    public class LiveChannelManagerDto 
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 关联视频Id
        /// </summary>
        public int VideoId { get; set; }

        /// <summary>
        /// 禁止视频循环播放
        /// </summary>
        public bool BannedVideoLoop { get; set; }

        /// <summary>
        /// 切换频道时间间隔
        /// </summary>
        public TimeSpan? PlayInterval { get; set; }

        /// <summary>
        /// 拉流切换输入流时间间隔
        /// </summary>
        public TimeSpan? PullInterval { get; set; }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 工厂代号
        /// </summary>
        public string FactoryCode { get; set; }

        /// <summary>
        /// 推流地址
        /// </summary>
        public string PushUrl { get; set; }

        public int Index { get; set; }

        /// <summary>
        /// 停止播放记录时间
        /// </summary>
        public DateTime StopTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新拉流信息的时间
        /// </summary>
        public DateTime UpdatePullInfoTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 拉流下标
        /// </summary>
        public int PullIndex { get; set; } = 0;
        


    }

    public class PullInfoManagerDto 
    {
        public int Id { get; set; }
        /// <summary>
        /// 频道Id
        /// </summary>
        public Guid ChannelId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

    }
}
