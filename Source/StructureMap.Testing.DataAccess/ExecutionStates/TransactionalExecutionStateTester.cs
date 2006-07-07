using System;
using System.Data;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.ExecutionStates;

namespace StructureMap.Testing.DataAccess.ExecutionStates
{
	[TestFixture]
	public class TransactionalExecutionStateTester
	{
		private IMock _connectionMock;
		private IMock _commandMock;
		private IMock _transactionMock;
		private IDbTransaction _transaction;
		private TransactionalExecutionState _executionState;
		private IDbConnection _connection;

		[SetUp]
		public void SetUp()
		{
			_connectionMock = new DynamicMock(typeof(IDbConnection));
			_connectionMock.ExpectAndReturn("State", ConnectionState.Closed);
			_commandMock = new DynamicMock(typeof(IDbCommand));

			_connection = (IDbConnection) _connectionMock.MockInstance;
			_executionState = new TransactionalExecutionState(_connection);

			_transactionMock = new DynamicMock(typeof(IDbTransaction));
			_transaction = (IDbTransaction) _transactionMock.MockInstance;
		}

		[Test]
		public void BeginTransaction()
		{
			beginTransaction();
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

		private void beginTransaction()
		{
			_connectionMock.Expect("Open");
			_connectionMock.ExpectAndReturn("BeginTransaction", _transaction);
	
			_executionState.BeginTransaction();
		}

		[Test]
		public void ExecuteHappyPath()
		{
			_commandMock.Expect("Connection", _connection);
			_commandMock.ExpectAndReturn("ExecuteNonQuery", 1);
			_commandMock.Expect("Connection", new IsNull());
			_connectionMock.Strict = true; // no calls to connection.
			

			int recordCount = _executionState.Execute((IDbCommand) _commandMock.MockInstance);
			Assert.AreEqual(1, recordCount);

			_commandMock.Verify();
			_connectionMock.Verify();
		}

		[Test]
		public void ExecuteWithAnException()
		{
			_commandMock.Expect("Connection", _connection);
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
