using JFJT.Framework.AutoMapper;
using JFJT.Framework.Domain.Entities;
using LiveCommand.Common;
using LiveFactory.Application.Command;
using LiveFactory.Core.Enums;
using LiveFactory.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using JFJT.Framework.Extensions;

namespace LiveFactory.Application
{
    [AutoMap(typeof(LiveChannel))]
    public class LiveChannelDto : CreateModifyAudited<Guid>
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

        public int Index { get; set; }

        /// <summary>
        /// 频道状态
        /// </summary>
        public LiveStateType State
        {
            get
            {
                var fist = LiveChannelStateManage.StateData.FirstOrDefault(b => b.Id == this.Id);
                return fist == null ? LiveStateType.Stoped : fist.State;
            }
        }
        /// <summary>
        /// 错误时播放地址
        /// </summary>
        public string ErrPlayUrl { get; set; }

        public string StateName
        {
            get
            {
                return this.State.ToEnumDesc();
            }
        }
        /// <summary>
        /// 前端处理图片变灰使用
        /// </summary>
        public string Style { get; set; }

        public bool IsDisabled { get { return false; } }


        public string ChannelDesc { get; set; }

        private static DateTime _starTime;
        public static DateTime StarTime { get { return DateTime.Now; } set { value = _starTime; } }

       
    }
}
