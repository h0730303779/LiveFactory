using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common.NetMQ.Models
{
    public interface IRequestData : ISocketData
    {
        RequestType RequestType { get; set; }
        string JsonData { get; set; }

    }
    public class RequestData : IRequestData
    {
        public RequestType RequestType { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public string JsonData { get; set; }
    }

  
}
