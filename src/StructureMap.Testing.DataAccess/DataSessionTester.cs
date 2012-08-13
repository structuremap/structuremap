using System;
using NMock;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.ExecutionStates;

namespace StructureMap.Testing.DataAccess
{
	[TestFixture]
	public class DataSessionTester
	{
		private IMock _factoryMock;
		private IMock _databaseMock;
		private DataSession _session;
		private IDatabaseEngine _database;
		private ICommandFactory _factory;
		private IMock _autoCommitMock;
		private IMock _transactionalMock;

		[SetUp]
		public void SetUp()
		{
			_factoryMock = new DynamicMock(typeof(ICommandFactory));
			_databaseMock = new DynamicMock(typeof(IDatabaseEngine));

			_database = (IDatabaseEngine)_databaseMock.MockInstance;
			_factory = (ICommandFactory)_factoryMock.MockInstance;
			
			_autoCommitMock = new DynamicMock(typeof(IExecutionState));
			_transactionalMock = new DynamicMock(typeof(ITransactionalExecutionState));


			IExecutionState autoCommitState = (IExecutionState) _autoCommitMock.MockInstance;
			ITransactionalExecutionState transactionalState = (ITransactionalExecutionState) _transactionalMock.MockInstance;

			_session = new DataSession(_database, _factory, autoCommitState, transactionalState);
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

		[Test, ExpectedException(typeof(ApplicationException), "A transaction is already started!")]
		public void CallingStartTransactionTwiceResultsInException()
		{
			_transactionalMock.Expect("BeginTransaction");

			_session.BeginTransaction();			
			_session.BeginTransaction();			
		}
	}
}
