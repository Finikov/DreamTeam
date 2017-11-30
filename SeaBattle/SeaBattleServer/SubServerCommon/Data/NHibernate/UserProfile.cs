namespace SeaBattleServer.SubServerCommon.Data.NHibernate
{
    public class UserProfile
    {
        public virtual int Id { get; set; }
        public virtual User UserId { get; set; }
        public virtual string Name { get; set; }
        public virtual int Level { get; set; }
        public virtual int Experience { get; set; }
        public virtual int Wins { get; set; }
        public virtual int Losses { get; set; }
        public virtual int Points { get; set; }
    }
}