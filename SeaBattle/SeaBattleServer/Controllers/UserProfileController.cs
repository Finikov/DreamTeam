using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SeaBattleFramework;
using SeaBattleServer.Models;
using SeaBattleServer.SubServerCommon;
using SeaBattleServer.SubServerCommon.Data.NHibernate;

namespace SeaBattleServer.Controllers
{
    [RoutePrefix("LoginServer")]
    public class UserProfileController : ApiController
    {
        [Route("GetUserProfile")]
        [HttpGet]
        public Message GetUserProfile([FromBody] Message msg)
        {
            try
            {
                var peerId = (Guid)msg.Parameters[(byte)ClientParameterCode.PeerId];
                //var peerId = Guid.Parse(msg.Parameters[(byte) ClientParameterCode.PeerId].ToString());
                var userId = Server.GetUser(peerId);

                using (var sessions = NHibernateHelper.OpenSession())
                {
                    using (var transaction = sessions.BeginTransaction())
                    {
                        var userList = sessions.QueryOver<UserProfile>().Where(u => u.UserId.Id == userId).List();
                        transaction.Commit();
                        if (userList.Count == 0)
                            return new Message
                            {
                                Parameters = msg.Parameters,
                                ReturnCode = (short) ErrorCode.OperationDenied,
                                DebugMessage = "User's profile wasn't found"
                            };
                        var para = new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.Profile, ClientUserProfile.MakeUserProfile(userList[0])}
                        };
                        return new Message {Parameters = para, ReturnCode = (short) ErrorCode.Ok};
                    }
                }
            }
            catch (Exception e)
            {
                return new Message
                {
                    ReturnCode = (short)ErrorCode.OperationInvalid,
                    DebugMessage = e.Message
                };
            }
        }
    }
}
