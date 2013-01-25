using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Examples
{
    public interface IRepository
    {
    }

    public class InMemoryRepository : IRepository
    {
    }

    public class DatabaseRepository : IRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
    }

    public static class RepositoryBootstrapper
    {
        public static void Bootstrap()
        {
            ObjectFactory.Initialize(x =>
            {
                // In this case, we need to specify the value of "connectionString" argument to
                // the constructor function
                x.For<DatabaseRepository>().Use<DatabaseRepository>()
                    .WithCtorArg("connectionString").EqualToAppSetting("connectionString");
            });

            ObjectFactory.Initialize(x =>
            {
                x.ForConcreteType<DatabaseRepository>().Configure
                    .WithCtorArg("connectionString").EqualToAppSetting("connectionString");
            });

            // Now, we can request an instance of DatabaseRepository, and
            // StructureMap knows to create a new object using the 
            // connection string from the AppSettings section from the App.config
            // file
            var repository = ObjectFactory.GetInstance<DatabaseRepository>();
        }
    }

    // WeirdLegacyRepository is some sort of Singleton that we 
    // can't create directly with a constructor function
    public class WeirdLegacyRepository : IRepository
    {
        private WeirdLegacyRepository()
        {
        }

        public static WeirdLegacyRepository Current { get; set; }
    }


    public class RepositoryRegistry : Registry
    {
        public RepositoryRegistry()
        {
            // First I'll specify the "default" Instance of IRepository
            For<IRepository>().TheDefaultIsConcreteType<InMemoryRepository>();

            // Now, I'll add three more Instances of IRepository
            For<IRepository>().AddInstances(x =>
            {
                // "NorthAmerica" is the concrete type DatabaseRepository with 
                // the connectionString pointed to the NorthAmerica database
                x.OfConcreteType<DatabaseRepository>().WithName("NorthAmerica")
                    .WithCtorArg("connectionString").EqualTo("database=NorthAmerica");

                // "Asia/Pacific" is the concrete type DatabaseRepository with 
                // the connectionString pointed to the AsiaPacific database
                x.OfConcreteType<DatabaseRepository>().WithName("Asia/Pacific")
                    .WithCtorArg("connectionString").EqualTo("database=AsiaPacific");

                // Lastly, the "Weird" instance is built by calling a specified 
                // Lambda (an anonymous delegate will work as well).
                x.ConstructedBy(() => WeirdLegacyRepository.Current).WithName("Weird");
            });
            /*
            // Example #1
            var container1 = new Container(new RepositoryRegistry());

            // Example #2
            var container2 = new Container(x =>
            {
                x.AddRegistry<RepositoryRegistry>();
            });

            // Example #3
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<RepositoryRegistry>();
            });


            ObjectFactory.Initialize(x =>
            {
                x.For<IRepository>().TheDefaultIsConcreteType<InMemoryRepository>();

                x.For<IRepository>().AddInstances(y =>
                {
                    y.OfConcreteType<DatabaseRepository>().WithName("NorthAmerica")
                        .WithCtorArg("connectionString").EqualTo("database=NorthAmerica");

                    y.OfConcreteType<DatabaseRepository>().WithName("Asia/Pacific")
                        .WithCtorArg("connectionString").EqualTo("database=AsiaPacific");

                    y.ConstructedBy(() => WeirdLegacyRepository.Current).WithName("Weird");
                });
            });
             */
        }
    }
}