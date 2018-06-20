using System;
using System.ComponentModel;
using JFJT.Framework.Domain.Entities;
using LiveFactory.Core.Enums;

namespace LiveFactory.Core.Models
{
    /// <summary>
    /// 直播频道
    /// </summary>
    public class LiveChannel:CreateModifyAudited<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 展示图片
        /// </summary>
        public string Image { get; set; }

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

        /// <summary>
        /// 频道排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 频道状态
        /// </summary>
        

        public string ErrPlayUrl { get; set; }

        public string ChannelDesc { get; set; }
    }
}
