using JFJT.Framework.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application
{
    public class LiveChannelSearchDto : PageRequest
    {
        public string KeyWord { get; set; }
    }
}
