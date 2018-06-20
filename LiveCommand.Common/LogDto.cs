using LiveCommand.Common.NetMQ;
using System;

namespace LiveCommand.Common
{
    public class LogDto
    {
        public LiveStateType LogType { get; set; }
        public string LogContent { get; set; }
        public Guid ChanneId { get; set; }
        public string FactoryCode { get; set; }
        public Guid Id { get; set; }

    }
}
