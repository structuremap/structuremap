using System;
using System.Data;
using System.Data.SqlClient;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.ExecutionStates;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess
{
    [TestFixture]
    public class DataSessionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _factoryMock = new DynamicMock(typeof (ICommandFactory));
            _databaseMock = new DynamicMock(typeof (IDatabaseEngine));

            _database = (IDatabaseEngine) _databaseMock.MockInstance;
            _factory = (ICommandFactory) _factoryMock.MockInstance;

            _autoCommitMock = new DynamicMock(typeof (IExecutionState));
            _transactionalMock = new DynamicMock(typeof (ITransactionalExecutionState));


            IExecutionState autoCommitState = (IExecutionState) _autoCommitMock.MockInstance;
            ITransactionalExecutionState transactionalState =
                (ITransactionalExecutionState) _transactionalMock.MockInstance;

            _session = new DataSession(_database, _factory, autoCommitState, transactionalState);
        }

        #endregion

        private IMock _factoryMock;
        private IMock _databaseMock;
        private DataSession _session;
        private IDatabaseEngine _database;
        private ICommandFactory _factory;
        private IMock _autoCommitMock;
        private IMock _transactionalMock;

        [Test, ExpectedException(typeof (ApplicationException), "A transaction is already started!")]
        public void CallingStartTransactionTwiceResultsInException()
        {
            _transactionalMock.Expect("BeginTransaction");

            _session.BeginTransaction();
            _session.BeginTransaction();
        }

        [Test]
        public void CommitTransaction()
        {
            _transactionalMock.Expect("BeginTransaction");
            _transactionalMock.Expect("CommitTransaction");

            _session.BeginTransaction();
            _session.CommitTransaction();

            Assert.IsFalse(_session.IsInTransaction);
            _transactionalMock.Verify();
        }

        [Test]
        public void ExecuteCommandAutoCommitHappyPath()
        {
            IDbCommand command = new SqlCommand();

            _autoCommitMock.ExpectAndReturn("Execute", 5, command);

            int count = _session.ExecuteCommand(command);
            Assert.AreEqual(5, count);

            _autoCommitMock.Verify();
        }

        [Test]
        public void ExecuteCommandAutoCommitWithException()
        {
            IDbCommand command = new SqlCommand();

            string messageText = "message1";
            _autoCommitMock.ExpectAndThrow("Execute", new ApplicationException(messageText), command);

            try
            {
                _session.ExecuteCommand(command);
                Assert.Fail("Should have thrown an exception.");
            }
            catch (CommandFailureException ex)
            {
                Assert.AreEqual(messageText, ex.InnerException.Message);
            }

            _autoCommitMock.Verify();
        }

        [Test]
        public void ExecuteDataSetHappyPath()
        {
            IDbCommand command = new SqlCommand();

            DataSet theDataSet = new DataSet();

            _autoCommitMock.ExpectAndReturn("ExecuteDataSet", theDataSet, command);

            DataSet dataSet = _session.ExecuteDataSet(command);

            Assert.IsTrue(ReferenceEquals(theDataSet, dataSet));
            _autoCommitMock.Verify();
        }

        [Test]
        public void ExecuteDataSetThrownException()
        {
            IDbCommand command = new SqlCommand();
            _autoCommitMock.ExpectAndThrow("ExecuteDataSet", new ApplicationException("I'm tired"), command);

            try
            {
                _session.ExecuteDataSet(command);
                Assert.Fail("Should have thrown an exception");
            }
            catch (CommandFailureException)
            {
                // All good
            }
            catch (Exception)
            {
                throw;
            }

            _autoCommitMock.Verify();
        }

        [Test]
        public void ExecuteReaderAutoCommitHappyPath()
        {
            IDbCommand command = new SqlCommand();
            TableDataReader reader = new TableDataReader(new DataTable());

            _autoCommitMock.ExpectAndReturn("ExecuteReader", reader, command);

            IDataReader actual = _session.ExecuteReader(command);

            Assert.AreSame(actual, reader);

            _autoCommitMock.Verify();
        }

        [Test]
        public void ExecuteReaderAutoCommitWithException()
        {
            IDbCommand command = new SqlCommand();

            string theMessage = "bad error";
            _autoCommitMock.ExpectAndThrow("ExecuteReader", new ApplicationException(theMessage),
                                           new IsTypeOf(typeof (IDbCommand)));

            try
            {
                _session.ExecuteReader(command);
                Assert.Fail("should have thrown an exception");
            }
            catch (CommandFailureException e)
            {
                Assert.AreEqual(theMessage, e.InnerException.Message);
            }

            _autoCommitMock.Verify();
        }


        [Test]
        public void ExecuteReaderBySqlAutoCommitHappyPath()
        {
            string sql = "sql command";
            TableDataReader reader = new TableDataReader(new DataTable());

            IDbCommand command = new SqlCommand();
            _databaseMock.ExpectAndReturn("GetCommand", command);

            _autoCommitMock.ExpectAndReturn("ExecuteReader", reader, command);

            IDataReader actual = _session.ExecuteReader(sql);

            Assert.AreSame(actual, reader);

            _autoCommitMock.Verify();
            _databaseMock.Verify();
            Assert.AreEqual(sql, command.CommandText);
            Assert.AreEqual(CommandType.Text, command.CommandType);
        }


        [Test]
        public void ExecuteScalarAutoCommitHappyPath()
        {
            IDbCommand command = new SqlCommand();
            object returnValue = new object();

            _autoCommitMock.ExpectAndReturn("ExecuteScalar", returnValue, command);

            object actual = _session.ExecuteScalar(command);

            Assert.AreSame(actual, returnValue);

            _autoCommitMock.Verify();
        }

        [Test]
        public void ExecuteScalarAutoCommitWithException()
        {
            IDbCommand command = new SqlCommand();

            string theMessage = "bad error";
            _autoCommitMock.ExpectAndThrow("ExecuteScalar", new ApplicationException(theMessage),
                                           new IsTypeOf(typeof (IDbCommand)));

            try
            {
                _session.ExecuteScalar(command);
                Assert.Fail("should have thrown an exception");
            }
            catch (CommandFailureException e)
            {
                Assert.AreEqual(theMessage, e.InnerException.Message);
            }

            _autoCommitMock.Verify();
        }

        [Test]
        public void ExecuteSql()
        {
            IDbCommand command = new SqlCommand();

            _databaseMock.ExpectAndReturn("GetCommand", command);

            _autoCommitMock.ExpectAndReturn("Execute", 5, command);

            string theSql = "sql statements";
            int count = _session.ExecuteSql(theSql);
            Assert.AreEqual(5, count);

            _autoCommitMock.Verify();
            _databaseMock.Verify();
            Assert.AreEqual(theSql, command.CommandText);
            Assert.AreEqual(CommandType.Text, command.CommandType);
        }

        [Test]
        public void IsInTransactionIsFalseBecauseStartTransactionHasNotBeenCalled()
        {
            Assert.IsFalse(_session.IsInTransaction);
        }

        [Test]
        public void IsInTransactionIsTrueBecauseStartTransactionHasBeenCalled()
        {
            _transactionalMock.Expect("BeginTransaction");

            _session.BeginTransaction();

            Assert.IsTrue(_session.IsInTransaction);
            _transactionalMock.Verify();
        }

        [Test]
        public void RollbackTransaction()
        {
            _transactionalMock.Expect("BeginTransaction");
            _transactionalMock.Expect("RollbackTransaction");

            _session.BeginTransaction();
            _session.RollbackTransaction();

            Assert.IsFalse(_session.IsInTransaction);
            _transactionalMock.Verify();
        }
    }
}