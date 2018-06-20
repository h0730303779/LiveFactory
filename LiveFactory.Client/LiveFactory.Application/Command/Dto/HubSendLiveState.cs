using LiveCommand.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Application.Command.Dto
{
    public class HubSendLiveState
    {
        public LiveStateType State { get; set; }       
        public Guid ChanneId { get; set; }
        public string FactoryCode { get; set; }
    }
}
