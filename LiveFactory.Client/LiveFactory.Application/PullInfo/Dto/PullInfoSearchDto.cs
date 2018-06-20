using JFJT.Framework.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application
{
    public class PullInfoSearchDto: PageRequest
    {
        public string KeyWord { get; set; }

        public Guid? ChannelId { get; set; }

    }
}
