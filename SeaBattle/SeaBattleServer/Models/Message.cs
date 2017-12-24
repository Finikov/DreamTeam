using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace SeaBattleServer.Models
{
    public class Message : SeaBattleFramework.IMessage
    {
        public Dictionary<byte, object> Parameters { get; set; }
        public string DebugMessage { get; set; }
        public short ReturnCode { get; set; }
    }
}