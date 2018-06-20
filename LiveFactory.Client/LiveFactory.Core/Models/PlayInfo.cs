using System;
using JFJT.Framework.Domain.Entities;
using LiveFactory.Core.Enums;

namespace LiveFactory.Core.Models
{
    /// <summary>
    /// 前端播放信息
    /// </summary>
    public class PlayInfo: CreateAudited
    {
        /// <summary>
        /// 频道Id
        /// </summary>
        public Guid ChannelId { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary> 
        /// 链接
        /// </summary>
        public string PlayUrl { get; set; }

        /// <summary>
        /// 播放类型
        /// </summary>
        public PlayType Type { get; set; }
    }
}
