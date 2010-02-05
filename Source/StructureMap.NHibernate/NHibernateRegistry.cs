using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using StructureMap.Configuration.DSL;
using NHConfiguration = NHibernate.Cfg.Configuration;

namespace StructureMap.NH
{
    public interface IConfigurationBuilder
    {
        NHConfiguration Build();
    }

    public class BasicConfigurationBuilder : IConfigurationBuilder
    {
        public NHConfiguration Build()
        {
            return new NHConfiguration();
        }
    }

    // Things that vary,


    public interface IRegistrySource
    {
        Registry GetRegistry();
    }


    internal class NHibernateRegistry : Registry
    {
        internal NHibernateRegistry()
        {
            For<IConfigurationBuilder>().Use<BasicConfigurationBuilder>();

            For<NHConfiguration>()
                .Use(c => c.GetInstance<IConfigurationBuilder>().Build());

            ForSingletonOf<ISessionFactory>().Use(
                c => { return c.GetInstance<NHConfiguration>().BuildSessionFactory(); });

            For<ISession>()
                .HybridHttpOrThreadLocalScoped()
                .Use(c => c.GetInstance<ISessionFactory>().OpenSession());
        }
    }


    public class NHibernateConfiguration : IRegistrySource
    {
        private readonly NHibernateRegistry _registry = new NHibernateRegistry();
        public InstanceScope SessionIsScopedAs { set { _registry.For<ISession>().LifecycleIs(value); } }
        
        
        
        public string ConnectionString
        {
            set
            {
                OverrideConfiguration(c => c.SetProperty("connection string value", value));
            }
        }

        Registry IRegistrySource.GetRegistry()
        {
            configureRegistry(_registry);

            return _registry;
        }

        internal virtual void configureRegistry(NHibernateRegistry registry)
        {
            // no-op
        }

        public void SessionPerWebRequest()
        {
            SessionIsScopedAs = InstanceScope.Hybrid;
        }

        public void RegisterOtherServices(Action<Registry> configure)
        {
            configure(_registry);
        }

        // register a modification of the Configuration to happen independently of how
        // Configuration is built
        public void OverrideConfiguration(Action<NHConfiguration> configure)
        {
            _registry.For<NHConfiguration>().OnCreationForAll(configure);
        }
    }


    public class FluentNHibernateConfiguration : NHibernateConfiguration
    {
        private readonly FluentConfiguration _configuration = Fluently.Configure();
        private IPersistenceConfigurer _database;

        public IPersistenceConfigurer SqlServer2005()
        {
            MsSqlConfiguration configuration = MsSqlConfiguration.MsSql2005;
            _database = configuration;

            return configuration;
        }

        public void IncludeThisAssembly()
        {
            throw new NotImplementedException();
        }

        public FluentConfiguration ConfigureNHibernate
        {
            get
            {
                return _configuration;
            }
        }

        internal override void configureRegistry(NHibernateRegistry registry)
        {
            // Validation and blow up if everything is not set?
            registry.For<NHConfiguration>().Use(_configuration.BuildConfiguration);
        }
    }

    public class WestonsConfiguration : FluentNHibernateConfiguration
    {
        public WestonsConfiguration()
        {
            SessionPerWebRequest();
            //ConnectionString = getConnectionString();


            //ConfigureNHibernate.Database(MsSqlConfiguration.MsSql2008.ShowSql).Mappings(x => x.AutoMappings);
            
            
            IncludeThisAssembly();
            
        }
    }


    public class ThingThatInitializes
    {
        public static void Bootstrap()
        {
            //ObjectFactory.Initialize(x => { x.AddRegistry(new WestonsConfiguration()); });
        }
    }
}