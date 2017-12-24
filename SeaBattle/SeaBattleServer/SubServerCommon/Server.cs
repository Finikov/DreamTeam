using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SeaBattle;
using SeaBattleServer.SubServerCommon.Data.NHibernate;
using SeaBattleServer.SubServerCommon.Exceptions;

namespace SeaBattleServer.SubServerCommon
{

    public static class Server
    {
        public static Dictionary<Guid, int> Users = new Dictionary<Guid, int>();
        public static List<Session> Sessions = new List<Session>();

        public static int GetUser(Guid peerId)
        {
            if (Users.TryGetValue(peerId, out int userId))
                return userId;
            throw new ServerException
            {
                ErrorCode = ServerErrorCode.InvalidUser,
                ErrorText = "User with peerId:" + peerId + " wasn't found"
            };
        }

        public static Guid AddUser(int userId)
        {
            Guid peerId = Guid.NewGuid();
            Users.Add(peerId, userId);
            return peerId;
        }

        public static Guid CreateNewSession()
        {
            Guid id = Guid.NewGuid();
            Session session = new Session(id);
            Sessions.Add(session);
            return id;
        }

        public static void CloseSession(Guid id)
        {
            var cnt = Sessions.RemoveAll(s => s.Id == id);
            if (cnt == 0)
                throw new ServerException
                {
                    ErrorCode = ServerErrorCode.InvalidSession,
                    ErrorText = "There isn't game session with id:" + id
                };
        }

        public static Session GetSession(Guid id)
        {
            var session = Sessions.Find(s => s.Id == id);
            if (session == null)
                throw new ServerException
                {
                    ErrorCode = ServerErrorCode.InvalidSession,
                    ErrorText = "There isn't game session with id:" + id
                };

            return session;
        }
    }
}