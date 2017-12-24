using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattleFramework
{
    public enum ClientCommandCode : byte
    {
        Login,
        Register,
        GetUserProfile,
        Logout,
        FindMatch,
        CheckSesssionStatus,
        Shot,
        CheckTurn
    }
}
