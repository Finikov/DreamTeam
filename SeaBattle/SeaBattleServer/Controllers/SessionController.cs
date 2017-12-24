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
                        ErrorCode = (short)ClientErrorCode.InvalidSessionId,
                        DebugMessage = "Session with sessionId:" + sessionId + " wasn't found"
                    };

                return new Message
                {
                    ReturnCode = (short)ClientReturnCode.CheckingSessionStatus,
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

        [Route("FillGrid")]
        [HttpGet]
        public Message FillGrid(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var peerId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.PeerId].ToString());
                var sessionId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.SessionId].ToString());

                if (!Server.Users.ContainsKey(peerId))
                    return new Message
                    {
                        ErrorCode = (short) ClientErrorCode.InvalidPeerId,
                        DebugMessage = "User with peerId:" + peerId + " wasn't found"
                    };

                var sessions = Server.Sessions.Where(s =>
                    s.Id == sessionId && (s.Game.Player1.PeerId == peerId || s.Game.Player2.PeerId == peerId)).ToList();
                if(sessions.Count <= 0)
                    return new Message
                    {
                        ErrorCode = (short)ClientErrorCode.InvalidSessionId,
                        DebugMessage = "Session with sessionId:" + sessionId + " wasn't found"
                    };

                if (msg.Parameters.ContainsKey((byte) ClientParameterCode.ListShips))
                {
                    var jsonListShips = msg.Parameters[(byte) ClientParameterCode.ListShips].ToString();
                    var ships = JsonConvert.DeserializeObject<List<Ship>>(jsonListShips);

                }
                else
                    sessions[0].Game.AutoFilling(peerId);

                var para = new Dictionary<byte, object>
                {
                    {(byte) ClientParameterCode.Grid, JsonConvert.SerializeObject(sessions[0].Game.GetGridInfo(peerId).Item1.GridCells)}
                };

                return new Message
                {
                    ReturnCode = (short) ClientReturnCode.GridIsFilling,
                    Parameters = para
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
                var point = JsonConvert.DeserializeObject<Point>(msg.Parameters[(byte)ClientParameterCode.ShotPosition].ToString());
                var sessionId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.SessionId].ToString());
                var session = Server.Sessions.Find(s => s.Id == sessionId);

                var field = session.Game.ShotPlayer(point, peerId);
                if (session.Game.Winner == Guid.Empty)
                    return new Message
                    {
                        ReturnCode = (short)ClientReturnCode.ShotResult,
                        Parameters = new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.EnemyGrid, JsonConvert.SerializeObject(field)}
                        }
                    };


                var mes = new Message()
                {
                    ReturnCode = (short)ClientReturnCode.Finish,
                    Parameters = new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.EnemyGrid, JsonConvert.SerializeObject(field)},
                        {(byte) ClientParameterCode.Winner, session.Game.Winner}
                    }
                };
                session.FinishSession();
                return mes;
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
                var peerId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.PeerId].ToString());
                var sessionId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.SessionId].ToString());
                var session = Server.Sessions.Find(s => s.Id == sessionId);

                if (session.Game.Winner == Guid.Empty)
                {
                    var para = new Dictionary<byte, object>
                    {
                        {
                            (byte) ClientParameterCode.Grid,
                            JsonConvert.SerializeObject(session.Game.GetGridInfo(peerId).Item1.GridCells)
                        }
                    };
                    if (session.Game.CurrentTurn == peerId)
                        return new Message
                        {
                            ReturnCode = (short)ClientReturnCode.GetGrid,
                            Parameters = para
                        };
                    return new Message
                    {
                        ReturnCode = (short)ClientReturnCode.WaitForYourTurn,
                        Parameters = para
                    };
                }

                var mes = new Message()
                {
                    ReturnCode = (short)ClientReturnCode.Finish,
                    Parameters = new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.Grid, JsonConvert.SerializeObject(session.Game.GetGridInfo(peerId).Item1.GridCells)},
                            {(byte) ClientParameterCode.Winner, session.Game.Winner}
                        }
                };
                session.FinishSession();
                return mes;
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

        [Route("CloseSession")]
        [HttpGet]
        public Message CloseSession(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var peerId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.PeerId].ToString());
                var sessionId = Guid.Parse(msg.Parameters[(byte)ClientParameterCode.SessionId].ToString());
                var session = Server.Sessions.Find(s => s.Id == sessionId);

                if (session.Status == GameStatus.Started)
                {
                    session.Game.Winner = (peerId == session.Game.Player1.PeerId)
                        ? session.Game.Player2.PeerId
                        : session.Game.Player1.PeerId;
                    var mes = new Message()
                    {
                        ReturnCode = (short)ClientReturnCode.Finish,
                        Parameters = new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.Grid, JsonConvert.SerializeObject(session.Game.GetGridInfo(peerId).Item1.GridCells)},
                            {(byte) ClientParameterCode.Winner, session.Game.Winner}
                        }
                    };
                    session.CloseSession();
                    return mes;

                }
                session.CloseSession();
                
                return new Message()
                {
                    ReturnCode = (short) ClientReturnCode.NoCode
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
