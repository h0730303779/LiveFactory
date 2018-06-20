using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common.NetMQ.Models
{
    public interface IResponseData : ISocketData
    {
        bool Success { get; set; }
        string Message { get; set; }
        string JsonData { get; set; }

    }
    public class ResponseData : IResponseData
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid Id { get; set; }
        public string JsonData { get; set; }
    }
}
