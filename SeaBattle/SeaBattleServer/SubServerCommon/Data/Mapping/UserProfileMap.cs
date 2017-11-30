using FluentNHibernate.Mapping;
using SeaBattleServer.SubServerCommon.Data.NHibernate;

namespace SeaBattleServer.SubServerCommon.Data.Mapping
{
    public class UserProfileMap : ClassMap<UserProfile>
    {
        public UserProfileMap()
        {
            Id(x => x.Id).Column("id");
            References(x => x.UserId).Column("user_id");
            Map(x => x.Name).Column("name");
            Map(x => x.Level).Column("level");
            Map(x => x.Experience).Column("experience");
            Map(x => x.Wins).Column("wins");
            Map(x => x.Losses).Column("losses");
            Map(x => x.Points).Column("points");
            Table("user_profile");
        }
    }
}