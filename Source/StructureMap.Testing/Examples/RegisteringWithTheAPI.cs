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
        public DatabaseRepository(string connectionString)
        {
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
            ForRequestedType<IRepository>().TheDefaultIsConcreteType<InMemoryRepository>();
            
            // Now, I'll add three more Instances of IRepository
            ForRequestedType<IRepository>().AddInstances(x =>
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
        }
    }
}