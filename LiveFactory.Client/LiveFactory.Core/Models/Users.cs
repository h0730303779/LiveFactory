using JFJT.Framework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Core.Models
{
    public class Users : CreateAudited<Guid>
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 最后登陆IP
        /// </summary>
        public string LastIP { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; } = false;
    }
}
