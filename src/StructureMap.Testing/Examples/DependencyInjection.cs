namespace StructureMap.Testing.Examples
{
    public class DependencyInjectionSample
    {
        // SAMPLE: basic-dependency-injection
        public interface IDatabase { }

        public class DatabaseUser
        {
            // Using Constructor Injection
            public DatabaseUser(IDatabase database)
            {
            }
        }

        public class OtherDatabaseUser
        {
            // Setter Injection
            public IDatabase Database { get; set; }
        }

        // ENDSAMPLE

        // SAMPLE: basic-service-location
        public class ThirdDatabaseUser
        {
            private IDatabase _database;

            public ThirdDatabaseUser(IContainer container)
            {
                // This is service location
                _database = container.GetInstance<IDatabase>();
            }
        }

        // ENDSAMPLE
    }
}