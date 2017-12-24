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

namespace SeaBattleServer.Controllers
{
    [RoutePrefix("GameServer")]
    public class MatchMakingController : ApiController
    {
        [Route("FindMatch")]
        [HttpGet]
        public Message FindMatch(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var peerId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.PeerId].ToString());
                if (!Server.Users.ContainsKey(peerId))
                    return new Message
                    {
                        ErrorCode = (short) ClientErrorCode.InvalidPeerId,
                        DebugMessage = "User with peerId:" + peerId + " wasn't found"
                    };

                var sessions = Server.Sessions.Where(s => s.Status == GameStatus.FindingOpponent).ToList();
                if (sessions.Count == 0)
                {
                    var sessionId = Server.CreateNewSession();
                    Server.Sessions.First(s => s.Id == sessionId).AddPlayer(peerId);
                    return new Message
                    {
                        ReturnCode = (short) ClientReturnCode.FindingOpponent,
                        Parameters = new Dictionary<byte, object> {{(byte) ClientParameterCode.SessionId, sessionId}}
                    };
                }

                sessions[0].AddPlayer(peerId);
                return new Message
                {
                    ReturnCode = (short) ClientReturnCode.GameWasFound,
                    Parameters = new Dictionary<byte, object> {{(byte) ClientParameterCode.SessionId, sessions[0].Id}}
                };
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
