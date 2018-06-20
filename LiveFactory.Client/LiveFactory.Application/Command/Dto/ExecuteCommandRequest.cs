using LiveCommand.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application.Command
{
    public class ExecuteCommandRequest
    {
        public Guid Id { get; set; }
        public CommandTypeEnum CommandType { get; set; }
    }
   
}
