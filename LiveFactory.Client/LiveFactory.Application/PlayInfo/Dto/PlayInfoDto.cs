using JFJT.Framework.AutoMapper;
using JFJT.Framework.Domain.Entities;
using LiveFactory.Core.Enums;
using LiveFactory.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application
{
    [AutoMap(typeof(PlayInfo))]
    public class PlayInfoDto : CreateAudited
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
