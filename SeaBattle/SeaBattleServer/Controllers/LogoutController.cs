using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using SeaBattleFramework;
using SeaBattleServer.Models;
using SeaBattleServer.SubServerCommon;
using SeaBattleServer.SubServerCommon.Data.NHibernate;

namespace SeaBattleServer.Controllers
{
    [RoutePrefix("LoginServer")]
    public class LogoutController : ApiController
    {
        [Route("Logout")]
        [HttpGet]
        public Message Logout(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var peerId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.PeerId].ToString());
                var sessions = Server.Sessions.Where(s => s.Game.Player1.PeerId == peerId || s.Game.Player2.PeerId == peerId).ToList();
                if (sessions.Count > 0)
                    foreach (var session in sessions)
                        session.RemovePlayer(peerId);

                Server.Users.Remove(peerId);
                return new Message {ReturnCode = (short) ClientReturnCode.NoCode};
            }
            catch (Exception e)
            {
                return new Message
                {
                    ErrorCode = (short)ClientErrorCode.OperationInvalid,
                    DebugMessage = e.Message
                };
            }
        }
    }
}
