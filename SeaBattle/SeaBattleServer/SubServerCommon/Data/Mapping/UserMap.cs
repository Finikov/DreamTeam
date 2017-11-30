using FluentNHibernate.Mapping;
using SeaBattleServer.SubServerCommon.Data.NHibernate;

namespace SeaBattleServer.SubServerCommon.Data.Mapping
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.Username).Column("username");
            Map(x => x.Password).Column("password");
            Map(x => x.Salt).Column("salt");
            Map(x => x.Algorithm).Column("algorithm");
            Map(x => x.Created).Column("created_at");
            Map(x => x.Updated).Column("updated_at");
            Table("user");
        }
    }
}