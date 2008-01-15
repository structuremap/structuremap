using System;
using System.Data;

namespace StructureMap.DataAccess.ExecutionStates
{
    public class AutoCommitExecutionState : IExecutionState
    {
        private readonly IDbDataAdapter _adapter;
        private readonly IDbConnection _connection;

        public AutoCommitExecutionState(IDbConnection connection, IDbDataAdapter adapter)
        {
            _connection = connection;
            _adapter = adapter;
        }

        #region IExecutionState Members

        public int Execute(IDbCommand command)
        {
            try
            {
                setupConnection(command);
                return command.ExecuteNonQuery();
            }
            finally
            {
                cleanupConnection(command);
            }
        }

        public IDataReader ExecuteReader(IDbCommand command)
        {
            try
            {
                setupConnection(command);
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                cleanupConnection(command);
                throw;
            }
        }

        public DataSet ExecuteDataSet(IDbCommand command)
        {
            try
            {
                setupConnection(command);
                _adapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                _adapter.Fill(dataSet);

                return dataSet;
            }
            finally
            {
                cleanupConnection(command);
            }
        }

        public object ExecuteScalar(IDbCommand command)
        {
            try
            {
                setupConnection(command);
                return command.ExecuteScalar();
            }
            finally
            {
                cleanupConnection(command);
            }
        }

        #endregion

        private void setupConnection(IDbCommand command)
        {
            command.Connection = _connection;
            _connection.Open();
        }

        private void cleanupConnection(IDbCommand command)
        {
            command.Connection = null;
            _connection.Close();
        }
    }
}