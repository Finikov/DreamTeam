using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using SeaBattleFramework;
using SeaBattleServer.Models;
using SeaBattleServer.SubServerCommon;
using SeaBattleServer.SubServerCommon.Data.NHibernate;

namespace SeaBattleServer.Controllers
{
    [RoutePrefix("LoginServer")]
    public class LoginController : ApiController
    {
        [Route("Login")]
        [HttpGet]
        public Message Login(string data)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(data);

                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var username = msg.Parameters[(byte)ClientParameterCode.Username].ToString();
                        var password = msg.Parameters[(byte)ClientParameterCode.Password].ToString();

                        var userList = session.QueryOver<User>().Where(u => u.Username == username).List();
                        if (userList.Count <= 0)
                        {
                            transaction.Commit();
                            return new Message
                            {
                                ReturnCode = (short) ErrorCode.IncorretcUsernameOrPassword,
                                DebugMessage = "Username or password is incorrect"
                            };
                        }

                        User user = userList[0];
                        var hash = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Salt + password)))
                            .Replace("-", "");

                        transaction.Commit();
                        if (String.Equals(hash.Trim(), user.Password.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            if (Server.ContainsUser(user.Id))
                                return new Message
                                {
                                    ReturnCode = (short) ErrorCode.UserCurrentlyLoggedIn,
                                    DebugMessage = "User is currently logged in"
                                };

                            var peerId = Server.AddUser(user.Id);
                            var para = new Dictionary<byte, object> {{(byte) ClientParameterCode.PeerId, peerId}};

                            return new Message {ReturnCode = (short) ErrorCode.Ok, Parameters = para};
                        }
                        return new Message
                        {
                            ReturnCode = (short) ErrorCode.IncorretcUsernameOrPassword,
                            DebugMessage = "Username or password is incorrect"
                        };
                    }
                }
            }
            catch (Exception e)
            {
                // TODO: добавить LogDebug 
                return new Message
                {
                    ReturnCode = (short) ErrorCode.OperationInvalid,
                    DebugMessage = e.Message
                };
            }
        }
    }
}
