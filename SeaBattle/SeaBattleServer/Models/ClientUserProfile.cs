using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SeaBattleFramework;
using SeaBattleServer.SubServerCommon.Data.NHibernate;

namespace SeaBattleServer.Models
{
    public class ClientUserProfile : IUserProfile
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }

        public static ClientUserProfile MakeUserProfile(UserProfile profile)
        {
            return new ClientUserProfile
            {
                Name = profile.Name,
                Level = profile.Level,
                Experience = profile.Experience,
                Wins = profile.Wins,
                Losses = profile.Losses,
                Points = profile.Points
            };
        }
    }
}