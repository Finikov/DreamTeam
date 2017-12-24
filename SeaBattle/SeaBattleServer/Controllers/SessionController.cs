using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web.Http;
using Newtonsoft.Json;
using SeaBattle;
using SeaBattleFramework;
using SeaBattleServer.Models;
using SeaBattleServer.SubServerCommon;

namespace SeaBattleServer.Controllers
{
    [RoutePrefix("GameServer")]
    public class SessionController : ApiController
    {
        [Route("CheckSesssionStatus")]
        [HttpGet]
        public Message CheckSesssionStatus(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var sessionId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.SessionId].ToString());
                var sessions = Server.Sessions.Where(s => s.Id == sessionId).ToList();

                if (sessions.Count == 0)
                    return new Message
                    {
                        ErrorCode = (short)ClientErrorCode.InvalidPeerId,
                        DebugMessage = "Session with sessionId:" + sessionId + " wasn't found"
                    };

                return new Message
                {
                    ReturnCode = (short) ClientReturnCode.CheckingSessionStatus,
                    Parameters = new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.SessionStatus, (short) sessions[0].Status}
                    }
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

        [Route("Shot")]
        [HttpGet]
        public Message Shot(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var peerId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.PeerId].ToString());
                var point = JsonConvert.DeserializeObject<Point>(msg.Parameters[(byte) ClientParameterCode.ShotPosition].ToString());
                var sessionId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.SessionId].ToString());
                var session = Server.Sessions.Find(s => s.Id == sessionId);

                var field = session.Game.ShotPlayer(point, peerId);

                return new Message
                {
                    ReturnCode = (short)ClientReturnCode.ShotResult,
                    Parameters = new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.EnemyGrid, field}
                    }
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

        [Route("CheckTurn")]
        [HttpGet]
        public Message CheckTurn(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var peerId = Guid.Parse(msg.Parameters[(byte) ClientParameterCode.PeerId].ToString());
                var sessionId = Guid.Parse(msg.Parameters[(byte) ClientParameterCode.SessionId].ToString());
                var session = Server.Sessions.Find(s => s.Id == sessionId);

                if (session.Game.CurrentTurn.PeerId == peerId)
                    return new Message
                    {
                        ReturnCode = (short) ClientReturnCode.Gridfield,
                        Parameters = new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.Grid, session.Game.GetGridInfo(peerId).Item1.GridCells}
                        }
                    };

                return new Message
                {
                    ReturnCode = (short) ClientReturnCode.WaitforYourTurn
                };
            }
            catch (Exception e)
            {
                return new Message
                {
                    ErrorCode = (short) ClientErrorCode.OperationInvalid,
                    DebugMessage = e.Message
                };
           }

        }
    }
}
