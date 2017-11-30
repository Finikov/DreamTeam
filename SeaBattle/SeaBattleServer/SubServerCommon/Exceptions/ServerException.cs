using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeaBattleServer.SubServerCommon.Exceptions
{
    public enum ServerErrorCode
    {
        InvalidUser,

    }

    public class ServerException : Exception
    {
        public ServerErrorCode ErrorCode { get; set; }
        public string ErrorText { get; set; }
    }
}