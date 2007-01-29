using System;
using System.Data;

namespace StructureMap.DataAccess.ExecutionStates
{
    public class TransactionalExecutionState : ITransactionalExecutionState
    {
        private readonly IDbConnection _connection;
        private readonly IDbDataAdapter _adapter;
        private IDbTransaction _transaction;

        public TransactionalExecutionState(IDbConnection connection, IDbDataAdapter adapter)
        {
            if (connection.State != ConnectionState.Closed)
            {
                throw new ArgumentOutOfRangeException("connection", "Connection cannot be open!");
            }

            _connection = connection;
            _adapter = adapter;
        }

        public int Execute(IDbCommand command)
        {
            try
            {
                prepareCommand(command);
                return command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection = null;
            }
        }

        private void prepareCommand(IDbCommand command)
        {
            command.Connection = _connection;
            command.Transaction = _transaction;
        }

        public IDataReader ExecuteReader(IDbCommand command)
        {
            prepareCommand(command);
            return command.ExecuteReader(CommandBehavior.Default);
        }

        public DataSet ExecuteDataSet(IDbCommand command)
        {
            try
            {
                prepareCommand(command);
                _adapter.SelectCommand = command;
                DataSet dataSet = new DataSet();

                _adapter.Fill(dataSet);

                return dataSet;
            }
            finally
            {
                _adapter.SelectCommand = null;
                command.Connection = null;
            }
        }

        public object ExecuteScalar(IDbCommand command)
        {
            try
            {
                prepareCommand(command);
                return command.ExecuteScalar();
            }
            finally
            {
                command.Connection = null;
            }
        }

        public void BeginTransaction()
        {
            try
            {
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception)
            {
                forceClose();
                throw;
            }
        }

        private void forceClose()
        {
            try
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }
            catch (Exception)
            {
                // Just trap the exception
            }
        }

        public void CommitTransaction()
        {
            try
            {
                _transaction.Commit();
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                throw new TransactionFailureException("Transaction failed!", ex);
            }
            finally
            {
                forceClose();
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _transaction.Rollback();
            }
            finally
            {
                forceClose();
            }
        }

        public void Dispose()
        {
            forceClose();
        }
    }
}