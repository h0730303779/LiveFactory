using System;
using System.Collections.Generic;
using System.Text;
using JFJT.Framework.Domain.Entities;
using LiveFactory.Core.Enums;

namespace LiveFactory.Core.Models
{
    /// <summary>
    /// 操作信息
    /// </summary>
    public class OperationInfo:CreateAudited
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public OperationType Type { get; set; }

        public string ParamValue { get; set; }

        /// <summary>
        /// 操作备注
        /// </summary>
        public string Remark { get; set; }
     
    }
}
