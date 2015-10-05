using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class custom_policies
    {
        public class DatabaseUser
        {
            public string ConnectionString { get; set; }

            public DatabaseUser(string connectionString)
            {
                ConnectionString = connectionString;
            }
        }

        public class ConnectedThing
        {
            public string ConnectionString { get; set; }

            public ConnectedThing(string connectionString)
            {
                ConnectionString = connectionString;
            }
        }

        public class ConnectionStringPolicy : ConfiguredInstancePolicy
        {
            protected override void apply(Type pluginType, IConfiguredInstance instance)
            {
                var parameter = instance.Constructor.GetParameters().FirstOrDefault(x => x.Name == "connectionString");
                if (parameter != null)
                {
                    var connectionString = findConnectionStringFromConfiguration();
                    instance.Dependencies.AddForConstructorParameter(parameter, connectionString);
                }
            }

            private string findConnectionStringFromConfiguration()
            {
                return "the connection string";
            }
        }

        [Test]
        public void use_the_connection_string_policy()
        {
            var container = new Container(_ =>
            {
                _.Policies.Add<ConnectionStringPolicy>();
            });

            container.GetInstance<DatabaseUser>()
                .ConnectionString.ShouldBe("the connection string");

            container.GetInstance<ConnectedThing>()
                .ConnectionString.ShouldBe("the connection string");
        }



        public interface IDatabase { }

        public class Database : IDatabase
        {
            public string ConnectionString { get; set; }

            public Database(string connectionString)
            {
                ConnectionString = connectionString;
            }

            public override string ToString()
            {
                return string.Format("ConnectionString: {0}", ConnectionString);
            }
        }

        public class BigService
        {
            public BigService(IDatabase green)
            {
                DB = green;
            }

            public IDatabase DB { get; set; }
        }

        public class ImportantService
        {
            public ImportantService(IDatabase red)
            {
                DB = red;
            }

            public IDatabase DB { get; set; }
        }

        public class DoubleDatabaseUser
        {
            public DoubleDatabaseUser(IDatabase red, IDatabase green)
            {
                Red = red;
                Green = green;
            }

            // Watch out for potential conflicts between setters
            // and ctor params. The easiest thing is to just make
            // setters private
            public IDatabase Green { get; private set; }
            public IDatabase Red { get; private set; }
        }

        public class InjectDatabaseByName : ConfiguredInstancePolicy
        {
            protected override void apply(Type pluginType, IConfiguredInstance instance)
            {
                instance.Constructor.GetParameters()
                    .Where(x => x.ParameterType == typeof (IDatabase))
                    .Each(param =>
                    {
                        var db = new ReferencedInstance(param.Name);
                        instance.Dependencies.AddForConstructorParameter(param, db);
                    });
            }
        }


        [Test]
        public void choose_database()
        {
            var container = new Container(_ =>
            {
                _.For<IDatabase>().Add<Database>().Named("red").Ctor<string>("connectionString").Is("*red*");
                _.For<IDatabase>().Add<Database>().Named("green").Ctor<string>("connectionString").Is("*green*");

                _.Policies.Add<InjectDatabaseByName>();
            });

            // ImportantService should get the "red" database
            container.GetInstance<ImportantService>()
                .DB.As<Database>().ConnectionString.ShouldBe("*red*");

            // BigService should get the "green" database
            container.GetInstance<BigService>()
                .DB.As<Database>().ConnectionString.ShouldBe("*green*");

            // DoubleDatabaseUser gets both
            var user = container.GetInstance<DoubleDatabaseUser>();

            user.Green.As<Database>().ConnectionString.ShouldBe("*green*");
            user.Red.As<Database>().ConnectionString.ShouldBe("*red*");
        }


        public interface IWidgets { }
        public class WidgetCache : IWidgets { }

        public class CacheIsSingleton : IInstancePolicy
        {
            public void Apply(Type pluginType, Instance instance)
            {
                if (instance.ReturnedType.Name.EndsWith("Cache"))
                {
                    instance.SetLifecycleTo<SingletonLifecycle>();
                }
            }
        }

        [Test]
        public void set_cache_to_singleton()
        {
            var container = new Container(_ =>
            {
                _.Policies.Add<CacheIsSingleton>();

                _.For<IWidgets>().Use<WidgetCache>();
            });

            // The policy is applied *only* at the time
            // that StructureMap creates a "build plan"
            container.GetInstance<IWidgets>()
                .ShouldBeTheSameAs(container.GetInstance<IWidgets>());

            // Now that the policy has executed, we 
            // can verify that WidgetCache is a singleton
            container.Model.For<IWidgets>().Default
                .Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }
    }
}