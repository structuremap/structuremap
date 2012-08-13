namespace StructureMap.DataAccess
{
    [Pluggable("Default")]
    public class DefaultConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string _connectionString;

        public DefaultConnectionStringProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region IConnectionStringProvider Members

        public string GetConnectionString()
        {
            return _connectionString;
        }

        #endregion
    }
}