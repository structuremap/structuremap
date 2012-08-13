using System.Data;

namespace StructureMap.DataAccess.ExecutionStates
{
    public interface IExecutionState
    {
        int Execute(IDbCommand command);
        IDataReader ExecuteReader(IDbCommand command);
        DataSet ExecuteDataSet(IDbCommand command);
        object ExecuteScalar(IDbCommand command);
    }
}