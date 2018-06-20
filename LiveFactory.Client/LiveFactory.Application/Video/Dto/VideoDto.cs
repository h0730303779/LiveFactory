using JFJT.Framework.AutoMapper;
using JFJT.Framework.Domain.Entities;
using LiveFactory.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application
{
    [AutoMap(typeof(Video))]
    public class VideoDto: CreateModifyAudited<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///是否上传七牛云
        /// </summary>
        public bool IsUpload { get; set; }
        /// <summary>
        ///是否默认播放
        /// </summary>
        public bool IsDefaultPlay { get; set; }
    }
}
