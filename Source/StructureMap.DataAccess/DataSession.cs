using System;
using System.Data;
using StructureMap.DataAccess.Commands;
using StructureMap.DataAccess.ExecutionStates;
using StructureMap.DataAccess.Parameterization;

namespace StructureMap.DataAccess
{
    [Pluggable("Default")]
    public class DataSession : IDataSession
    {
        private readonly IDatabaseEngine _database;
        private readonly ICommandFactory _factory;
        private readonly IExecutionState _defaultState;
        private readonly ITransactionalExecutionState _transactionalState;
        private IExecutionState _currentState;
        private readonly ICommandCollection _commands;
        private readonly ReaderSourceCollection _readerSources;

        [DefaultConstructor]
        public DataSession(IDatabaseEngine database)
            : this(database,
                   new CommandFactory(database),
                   new AutoCommitExecutionState(database.GetConnection(), database.GetDataAdapter()),
                   new TransactionalExecutionState(database.GetConnection(), database.GetDataAdapter()))
        {
        }

        /// <summary>
        /// Testing constructor
        /// </summary>
        /// <param name="database"></param>
        /// <param name="factory"></param>
        /// <param name="defaultState"></param>
        /// <param name="transactionalState"></param>
        public DataSession(IDatabaseEngine database, ICommandFactory factory, IExecutionState defaultState,
                           ITransactionalExecutionState transactionalState)
        {
            _database = database;
            _factory = factory;
            _defaultState = defaultState;
            _transactionalState = transactionalState;

            _currentState = _defaultState;

            _commands = new CommandCollection(this, _factory);
            _readerSources = new ReaderSourceCollection(this, _factory);
        }

        public bool IsInTransaction
        {
            get { return (_currentState is ITransactionalExecutionState); }
        }

        public void BeginTransaction()
        {
            if (IsInTransaction)
            {
                throw new ApplicationException("A transaction is already started!");
            }

            _transactionalState.BeginTransaction();
            _currentState = _transactionalState;
        }

        public void CommitTransaction()
        {
            if (!IsInTransaction)
            {
                throw new ApplicationException("A transaction is not started!");
            }

            _transactionalState.CommitTransaction();
            _currentState = _defaultState;
        }

        public void RollbackTransaction()
        {
            if (!IsInTransaction)
            {
                throw new ApplicationException("A transaction is not started!");
            }

            _transactionalState.RollbackTransaction();
            _currentState = _defaultState;
        }


        public int ExecuteCommand(IDbCommand command)
        {
            try
            {
                return _currentState.Execute(command);
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(command, ex);
            }
        }

        public int ExecuteSql(string sql)
        {
            IDbCommand command = createCommand(sql);
            return ExecuteCommand(command);
        }

        private IDbCommand createCommand(string sql)
        {
            IDbCommand command = _database.GetCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            return command;
        }

        public IDataReader ExecuteReader(IDbCommand command)
        {
            try
            {
                return _currentState.ExecuteReader(command);
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(command, ex);
            }
        }

        public IDataReader ExecuteReader(string sql)
        {
            IDbCommand command = createCommand(sql);
            return ExecuteReader(command);
        }

        public DataSet ExecuteDataSet(IDbCommand command)
        {
            try
            {
                return _currentState.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(command, ex);
            }
        }

        public DataSet ExecuteDataSet(string sql)
        {
            IDbCommand command = createCommand(sql);
            return ExecuteDataSet(command);
        }

        public object ExecuteScalar(IDbCommand command)
        {
            try
            {
                return _currentState.ExecuteScalar(command);
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(command, ex);
            }
        }

        public object ExecuteScalar(string sql)
        {
            IDbCommand command = _database.GetCommand();
            command.CommandText = sql;
            return ExecuteScalar(command);
        }

        public ICommandCollection Commands
        {
            get { return _commands; }
        }

        public IReaderSourceCollection ReaderSources
        {
            get { return _readerSources; }
        }

        public IReaderSource CreateReaderSource(StructureMapCommandType commandType, string commandText)
        {
            return createCommandBase(commandType, commandText);
        }

        public ICommand CreateCommand(StructureMapCommandType commandType, string commandText)
        {
            return createCommandBase(commandType, commandText);
        }

        private CommandBase createCommandBase(StructureMapCommandType commandType, string commandText)
        {
            CommandBase returnValue = null;

            switch (commandType)
            {
                case StructureMapCommandType.Parameterized:
                    returnValue = new ParameterizedCommand(commandText);
                    break;

                case StructureMapCommandType.Templated:
                    returnValue = new TemplatedCommand(commandText, _database);
                    break;

                case StructureMapCommandType.StoredProcedure:
                    returnValue = new StoredProcedureCommand(commandText, this);
                    break;
            }

            returnValue.Attach(this);

            return returnValue;
        }

        public void Initialize(IInitializable initializable)
        {
            initializable.Initialize(_database);
        }

        [ValidationMethod]
        public void ValidateConnection()
        {
            using (IDbConnection connection = _database.GetConnection())
            {
                connection.Open();
            }
        }
    }
}