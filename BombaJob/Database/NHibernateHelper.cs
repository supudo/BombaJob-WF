using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;

using BombaJob.Database.Domain;
using NHibernate.Tool.hbm2ddl;

namespace BombaJob.Database
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure();
                    configuration.AddAssembly(System.Reflection.Assembly.GetCallingAssembly());

                    if (!System.IO.File.Exists(AppSettings.SQLiteFile))
                        new SchemaExport(configuration).Execute(true, true, false);

                    //SchemaUpdate dbSchema = new SchemaUpdate(configuration);
                    //dbSchema.Execute(false, true);

                    _sessionFactory = configuration.BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
