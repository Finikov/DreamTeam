using System.Collections.Generic;
using SeaBattleFramework;

namespace SeaBattleServer.Models
{
    public class Message : SeaBattleFramework.IMessage
    {
        public Dictionary<byte, object> Parameters { get; set; }
        public string DebugMessage { get; set; }
        public short ReturnCode { get; set; } = (short) ClientReturnCode.NoCode;
        public short ErrorCode { get; set; } = (short) ClientErrorCode.Ok;
    }
}