using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattleFramework
{
    public static class ServerInfo
    {
        public static string ServerUrl = "http://localhost:65172";
        public static Dictionary<byte, string> SubServerName = new Dictionary<byte, string>
        {
            {(byte) ClientSubServerCode.LoginServer, "LoginServer"},
            {(byte) ClientSubServerCode.GameServer, "GameServer"}
        };
    }
}
