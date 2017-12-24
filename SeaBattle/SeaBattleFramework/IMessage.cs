using System.Collections.Generic;

namespace SeaBattleFramework
{
    public interface IMessage
    {
        Dictionary<byte, object> Parameters { get; set; }
        string DebugMessage { get; set; }
        short ReturnCode { get; set; }
    }
}
