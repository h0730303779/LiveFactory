using System;
using JFJT.Framework.Domain.Entities;
using LiveFactory.Core.Enums;

namespace LiveFactory.Core.Models
{
    /// <summary>
    /// 拉流信息
    /// </summary>
    public class PullInfo:CreateAudited
    {
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

        /// <summary>
        /// 拉流类型
        /// </summary>
        public PullType Type { get; set; }
    }
}
