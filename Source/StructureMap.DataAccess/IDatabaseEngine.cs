using System.Data;

namespace StructureMap.DataAccess
{
    [PluginFamily("MSSQL")]
    public interface IDatabaseEngine
    {
        IDbConnection GetConnection();
        IDbCommand GetCommand();
        IDbCommand CreateStoredProcedureCommand(string commandText);
        IDbDataAdapter GetDataAdapter();


        IDataParameter CreateStringParameter(string parameterName, int size, bool isNullable);
        IDataParameter CreateParameter(string parameterName, DbType dbType, bool isNullable);
        string GetParameterName(string logicalName);
    }

    
}