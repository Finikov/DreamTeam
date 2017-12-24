using System;
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
    public class RegisterController : ApiController
    {
        [Route("Register")]
        [HttpGet]
        public Message Register(string data)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<Message>(data);
                var username = msg.Parameters[(byte)ClientParameterCode.Username].ToString();
                var password = msg.Parameters[(byte)ClientParameterCode.Password].ToString();

                if(username == "" || password == "")
                    return new Message
                    {
                        ReturnCode = (short)ErrorCode.OperationDenied,
                        DebugMessage = "All fields are required"
                    };

                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var userList = session.QueryOver<User>().Where(u => u.Username == username).List();
                        if (userList.Count > 0)
                        {
                            transaction.Commit();
                            return new Message
                            {
                                ReturnCode = (short) ErrorCode.UsernameInUse,
                                DebugMessage = "Account name already in use"
                            };
                        }

                        var salt = Guid.NewGuid().ToString().Replace("-", "");
                        var user = new User
                        {
                            Algorithm = "sha1",
                            Username = username,
                            Password = BitConverter
                                .ToString(SHA1.Create()
                                    .ComputeHash(Encoding.UTF8.GetBytes(salt + password))).Replace("-", ""),
                            Salt = salt,
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        };
                        session.Save(user);
                        transaction.Commit();
                    }
                    using(var transaction = session.BeginTransaction())
                    {
                        var userList = session.QueryOver<User>().Where(u => u.Username == username).List();
                        if (userList.Count > 0)
                        {
                            UserProfile profile = new UserProfile {UserId = userList[0], Name = userList[0].Username};
                            session.Save(profile);
                            transaction.Commit();
                        }
                    }
                }
                return new Message { ReturnCode = (short)ClientReturnCode.UserCreated };
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
