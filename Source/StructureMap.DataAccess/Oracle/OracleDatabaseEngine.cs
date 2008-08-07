using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Text;

namespace StructureMap.DataAccess.Oracle
{
    public class OracleDatabaseEngine : IDatabaseEngine
    {
        private IConnectionStringProvider _provider;

        public OracleDatabaseEngine(IConnectionStringProvider provider)
        {
            _provider = provider;
        }

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
            throw new System.NotImplementedException();
        }

        public IDbDataAdapter GetDataAdapter()
        {
            return new OracleDataAdapter();
        }

        public IDataParameter CreateStringParameter(string parameterName, int size, bool isNullable)
        {
            throw new System.NotImplementedException();
        }

        public IDataParameter CreateParameter(string parameterName, DbType dbType, bool isNullable)
        {
            throw new System.NotImplementedException();
        }

        public string GetParameterName(string logicalName)
        {
            throw new System.NotImplementedException();
        }
    }
}
