using LiveFactory.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using JFJT.Framework.Extensions;

namespace LiveFactory.Application.Dto
{
    public class LiveChannelWebDto
    {

        /// <summary>
        /// 名称
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 展示图片
        /// </summary>
        public string Image { get; set; }

        public string PlayUrl { get; set; }
        public string ErrPlayUrl { get; set; }
        public PlayType Type { get; set; }

        public string ChannelDesc { get; set; }

        public string TypeDesc
        {
            get
            {
                return Type.ToEnumDesc();
            }
        }
    }
}
