using System;
using System.Data;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class MockDataSession : IDataSession
    {
        private readonly StubbedCommandCollection _commands;
        private readonly StubbedReaderSourceCollection _sources;

        public MockDataSession()
        {
            _sources = new StubbedReaderSourceCollection();
            _commands = new StubbedCommandCollection();
        }

        #region IDataSession Members

        public bool IsInTransaction
        {
            get { throw new NotImplementedException(); }
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        public int ExecuteCommand(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public int ExecuteSql(string sql)
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(string sql)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteDataSet(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteDataSet(string sql)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string sql)
        {
            throw new NotImplementedException();
        }

        public ICommandCollection Commands
        {
            get { return _commands; }
        }

        public IReaderSourceCollection ReaderSources
        {
            get { return _sources; }
        }

        public IReaderSource CreateReaderSource(StructureMapCommandType commandType, string commandText)
        {
            throw new NotImplementedException();
        }

        public ICommand CreateCommand(StructureMapCommandType commandType, string commandText)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IInitializable initializable)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void AddReaderExpectation(string name, ReaderExpectation expectation)
        {
            var source = (MockReaderSource) _sources[name];
            source.AddExpectation(expectation);
        }


        public void AddCommandExpectation(string name, CommandExpectation expectation)
        {
            var command = (MockCommand) _commands[name];
            command.AddExpectation(expectation);
        }
    }
}