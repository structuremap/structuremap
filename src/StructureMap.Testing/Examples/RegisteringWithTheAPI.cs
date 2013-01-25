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

}