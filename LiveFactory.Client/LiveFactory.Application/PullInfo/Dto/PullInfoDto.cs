using JFJT.Framework.Domain.Entities;
using LiveFactory.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application
{
    public class PullInfoDto : CreateAudited
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
