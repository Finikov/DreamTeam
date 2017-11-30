using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using SeaBattle;
using SeaBattleServer.SubServerCommon.Data.NHibernate;

namespace SeaBattleServer.SubServerCommon
{
    public class NHibernateHelper
    {
        public NHibernateHelper()
        {
            InitializeSessionFactory();
        }

        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        private static void InitializeSessionFactory()
        {
            _sessionFactory = Fluently.Configure()
                .Database(
                    MySQLConfiguration.Standard
                        .ConnectionString(cs => cs.Server("localhost")
                            .Database("dreamteam")
                            .Username("Finikov")
                            .Password("Black1278")))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateHelper>())
                .BuildSessionFactory();
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}