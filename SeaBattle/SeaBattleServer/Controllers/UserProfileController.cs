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
    public class UserProfileController : ApiController
    {
        [Route("GetUserProfile")]
        [HttpGet]
        public Message GetUserProfile(string data)
        {
            try
            {
                //var peerId = (Guid)msg.Parameters[(byte)ClientParameterCode.PeerId];
                Message msg = JsonConvert.DeserializeObject<Message>(data);
                var peerId = Guid.Parse(msg.Parameters[(byte) ClientParameterCode.PeerId].ToString());
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
                                ErrorCode = (short) ClientErrorCode.OperationDenied,
                                DebugMessage = "User's profile wasn't found"
                            };

                        var para = new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.Profile, JsonConvert.SerializeObject(ClientUserProfile.MakeUserProfile(userList[0]))}
                        };
                        return new Message {Parameters = para, ReturnCode = (short) ClientReturnCode.ProfileSended};
                    }
                }
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
