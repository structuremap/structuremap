using System;
using System.Data;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.ExecutionStates;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess.ExecutionStates
{
    [TestFixture]
    public class TransactionalExecutionStateTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _connectionMock = new DynamicMock(typeof (IDbConnection));
            _connectionMock.ExpectAndReturn("State", ConnectionState.Closed);
            _commandMock = new DynamicMock(typeof (IDbCommand));

            _adapterMock = new DynamicMock(typeof (IDbDataAdapter));
            _connection = (IDbConnection) _connectionMock.MockInstance;
            _executionState = new TransactionalExecutionState(_connection, (IDbDataAdapter) _adapterMock.MockInstance);

            _transactionMock = new DynamicMock(typeof (IDbTransaction));
            _transaction = (IDbTransaction) _transactionMock.MockInstance;
        }

        #endregion

        private IMock _connectionMock;
        private IMock _commandMock;
        private IMock _transactionMock;
        private IDbTransaction _transaction;
        private TransactionalExecutionState _executionState;
        private IDbConnection _connection;
        private IMock _adapterMock;

        private void beginTransaction()
        {
            _connectionMock.Expect("Open");
            _connectionMock.ExpectAndReturn("BeginTransaction", _transaction);

            _executionState.BeginTransaction();
        }

        [Test]
        public void BeginTransaction()
        {
            beginTransaction();
            _connectionMock.Verify();
        }

        [Test]
        public void CommitTransactionExceptionOccurred()
        {
            beginTransaction();

            _transactionMock.ExpectAndThrow("Commit", new ApplicationException("Okay"));
            _connectionMock.Expect("Close");
            _connectionMock.ExpectAndReturn("State", ConnectionState.Open);
            _transactionMock.Expect("Rollback");

            try
            {
                _executionState.CommitTransaction();
                Assert.Fail("should have thrown exception");
            }
            catch (TransactionFailureException)
            {
                // just trap
            }

            _transactionMock.Verify();
            _connectionMock.Verify();
        }

        [Test]
        public void CommitTransactionHappyPath()
        {
            beginTransaction();

            _transactionMock.Expect("Commit");
            _connectionMock.Expect("Close");
            _connectionMock.ExpectAndReturn("State", ConnectionState.Open);

            _executionState.CommitTransaction();

            _transactionMock.Verify();
            _connectionMock.Verify();
        }

        [Test, Ignore("Problem with mocking IDbDataAdapter.Fill()")]
        public void ExecuteDataSetHappyPath()
        {
            _connectionMock.Expect("Open");
            _commandMock.Expect("Connection", _connectionMock.MockInstance);
            _commandMock.Expect("Connection", new IsNull());
            _adapterMock.Expect("Fill", new IsTypeOf(typeof (DataSet)));
            _adapterMock.Expect("SelectCommand", _commandMock.MockInstance);
            _adapterMock.Expect("SelectCommand", new IsNull());

            IDbCommand command = (IDbCommand) _commandMock.MockInstance;
            DataSet dataSet = _executionState.ExecuteDataSet(command);

            Assert.IsNotNull(dataSet);

            _connectionMock.Verify();
            _commandMock.Verify();
            _adapterMock.Verify();
        }

        [Test]
        public void ExecuteDataSetThrowsException()
        {
            _commandMock.Expect("Connection", _connectionMock.MockInstance);
            _commandMock.Expect("Connection", new IsNull());
            _adapterMock.ExpectAndThrow("Fill", new ApplicationException("Okay"), new IsTypeOf(typeof (DataSet)));
            _adapterMock.Expect("SelectCommand", _commandMock.MockInstance);
            _adapterMock.Expect("SelectCommand", new IsNull());

            try
            {
                _executionState.ExecuteDataSet((IDbCommand) _commandMock.MockInstance);
                Assert.Fail("Should have thrown the exception");
            }
            catch (ApplicationException e)
            {
                Assert.AreEqual("Okay", e.Message);
            }

            _commandMock.Verify();
            _connectionMock.Verify();
            _adapterMock.Verify();
        }

        [Test]
        public void ExecuteHappyPath()
        {
            beginTransaction();

            _commandMock.Expect("Connection", _connection);
            _commandMock.Expect("Transaction", _transaction);
            _commandMock.ExpectAndReturn("ExecuteNonQuery", 1);
            _commandMock.Expect("Connection", new IsNull());
            _connectionMock.Strict = true; // no calls to connection.


            int recordCount = _executionState.Execute((IDbCommand) _commandMock.MockInstance);
            Assert.AreEqual(1, recordCount);

            _commandMock.Verify();
            _connectionMock.Verify();
        }


        [Test]
        public void ExecuteReaderHappyPath()
        {
            beginTransaction();

            _commandMock.Expect("Connection", _connection);
            _commandMock.Expect("Transaction", _transaction);
            TableDataReader reader = new TableDataReader(new DataTable());
            _commandMock.ExpectAndReturn("ExecuteReader", reader, CommandBehavior.Default);
            _connectionMock.Strict = true; // no calls to connection.


            IDataReader actual = _executionState.ExecuteReader((IDbCommand) _commandMock.MockInstance);
            Assert.AreSame(reader, actual);

            _commandMock.Verify();
            _connectionMock.Verify();
        }


        [Test]
        public void ExecuteReaderWithException()
        {
            beginTransaction();

            _commandMock.Expect("Connection", _connection);
            _commandMock.Expect("Transaction", _transaction);
            _commandMock.ExpectAndThrow("ExecuteReader", new ApplicationException("okay"));
            _connectionMock.Strict = true; // no calls to connection.

            try
            {
                _executionState.ExecuteReader((IDbCommand) _commandMock.MockInstance);
                Assert.Fail("should be an exception here.");
            }
            catch (ApplicationException e)
            {
                Assert.AreEqual("okay", e.Message);
            }

            _commandMock.Verify();
            _connectionMock.Verify();
        }


        [Test]
        public void ExecuteScalarHappyPath()
        {
            beginTransaction();

            _commandMock.Expect("Connection", _connection);
            _commandMock.Expect("Transaction", _transaction);
            object returnValue = new object();
            _commandMock.ExpectAndReturn("ExecuteScalar", returnValue);
            _commandMock.Expect("Connection", new IsNull());
            _connectionMock.Strict = true; // no calls to connection.


            object actual = _executionState.ExecuteScalar((IDbCommand) _commandMock.MockInstance);
            Assert.AreSame(returnValue, actual);

            _commandMock.Verify();
            _connectionMock.Verify();
        }


        [Test]
        public void ExecuteScalarWithException()
        {
            beginTransaction();

            _commandMock.Expect("Connection", _connection);
            _commandMock.Expect("Transaction", _transaction);
            _commandMock.ExpectAndThrow("ExecuteScalar", new ApplicationException("okay"));
            _commandMock.Expect("Connection", new IsNull());
            _connectionMock.Strict = true; // no calls to connection.

            try
            {
                _executionState.ExecuteScalar((IDbCommand) _commandMock.MockInstance);
                Assert.Fail("should be an exception here.");
            }
            catch (ApplicationException e)
            {
                Assert.AreEqual("okay", e.Message);
            }

            _commandMock.Verify();
            _connectionMock.Verify();
        }

        [Test]
        public void ExecuteWithAnException()
        {
            beginTransaction();

            _commandMock.Expect("Connection", _connection);
            _commandMock.Expect("Transaction", _transaction);
            _commandMock.ExpectAndThrow("ExecuteNonQuery", new ApplicationException("okay"));
            _commandMock.Expect("Connection", new IsNull());
            _connectionMock.Strict = true; // no calls to connection.

            try
            {
                int recordCount = _executionState.Execute((IDbCommand) _commandMock.MockInstance);
                Assert.Fail("should be an exception here.");
            }
            catch (ApplicationException e)
            {
                Assert.AreEqual("okay", e.Message);
            }

            _commandMock.Verify();
            _connectionMock.Verify();
        }

        [Test]
        public void RollbackHappyPath()
        {
            beginTransaction();

            _transactionMock.Expect("Rollback");
            _connectionMock.ExpectAndReturn("State", ConnectionState.Open);

            _executionState.RollbackTransaction();

            _connectionMock.Verify();
            _transactionMock.Verify();
        }

        [Test]
        public void RollbackWithError()
        {
            beginTransaction();

            _transactionMock.ExpectAndThrow("Rollback", new ApplicationException());
            _connectionMock.ExpectAndReturn("State", ConnectionState.Open);

            try
            {
                _executionState.RollbackTransaction();
                Assert.Fail("Should have thrown the exception");
            }
            catch (Exception)
            {
                // should have thrown an Exception
            }

            _connectionMock.Verify();
            _transactionMock.Verify();
        }
    }
}