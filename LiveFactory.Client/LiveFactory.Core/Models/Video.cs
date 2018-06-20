using System;
using JFJT.Framework.Domain.Entities;

namespace LiveFactory.Core.Models
{
    /// <summary>
    /// 视频
    /// </summary>
    public class Video:CreateAudited<Guid>
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
