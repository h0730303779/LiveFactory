using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common.NetMQ.Models
{
    public interface ISocketData
    {
        Guid Id { get; set; }
    }
    public class SocketData : ISocketData
    {
        public virtual Guid Id { get; set; } = Guid.NewGuid();
    }
}
