using System;
using System.Data;
using System.Data.OracleClient;

namespace StructureMap.DataAccess.Oracle
{
    public class OracleDatabaseEngine : IDatabaseEngine
    {
        private readonly IConnectionStringProvider _provider;

        public OracleDatabaseEngine(IConnectionStringProvider provider)
        {
            _provider = provider;
        }

        #region IDatabaseEngine Members

        public IDbConnection GetConnection()
        {
            return new OracleConnection(_provider.GetConnectionString());
        }

        public IDbCommand GetCommand()
        {
            return new OracleCommand();
        }

        public IDbCommand CreateStoredProcedureCommand(string commandText)
        {
            throw new NotImplementedException();
        }

        public IDbDataAdapter GetDataAdapter()
        {
            return new OracleDataAdapter();
        }

        public IDataParameter CreateStringParameter(string parameterName, int size, bool isNullable)
        {
            throw new NotImplementedException();
        }

        public IDataParameter CreateParameter(string parameterName, DbType dbType, bool isNullable)
        {
            throw new NotImplementedException();
        }

        public string GetParameterName(string logicalName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}