using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattleFramework
{
    public interface IUserProfile
    {
        string Name { get; set; }
        int Level { get; set; }
        int Experience { get; set; }
        int Wins { get; set; }
        int Losses { get; set; }
        int Points { get; set; }
    }
}
