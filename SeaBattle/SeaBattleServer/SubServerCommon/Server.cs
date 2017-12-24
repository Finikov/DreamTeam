using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SeaBattleServer.SubServerCommon.Data.NHibernate;
using SeaBattleServer.SubServerCommon.Exceptions;

namespace SeaBattleServer.SubServerCommon
{
    public static class Server
    {
        private static Dictionary<Guid, int> _users = new Dictionary<Guid, int>();

        public static int GetUser(Guid peerId)
        {
            if (_users.TryGetValue(peerId, out int userId))
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
            _users.Add(peerId, userId);
            return peerId;
        }

        public static bool ContainsUser(Guid peerId)
        {
            return _users.ContainsKey(peerId);
        }

        public static bool ContainsUser(int userId)
        {
            return _users.ContainsValue(userId);
        }
    }
}