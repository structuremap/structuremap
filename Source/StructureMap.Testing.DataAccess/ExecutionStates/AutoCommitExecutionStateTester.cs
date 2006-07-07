using System;
using System.Data;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using StructureMap.DataAccess.ExecutionStates;

namespace StructureMap.Testing.DataAccess.ExecutionStates
{
	[TestFixture]
	public class AutoCommitExecutionStateTester
	{
		private IMock _connectionMock;
		private IMock _commandMock;
		private AutoCommitExecutionState _executionState;

		[SetUp]
		public void SetUp()
		{
			_connectionMock = new DynamicMock(typeof(IDbConnection));
			_commandMock = new DynamicMock(typeof(IDbCommand));

			_executionState = new AutoCommitExecutionState((IDbConnection) _connectionMock.MockInstance);
		}


		[Test]
		public void ExecuteHappyPath()
		{
			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_commandMock.ExpectAndReturn("ExecuteNonQuery", 1);
			_commandMock.Expect("Connection", new IsNull());
			_connectionMock.Expect("Close");

			IDbCommand command = (IDbCommand) _commandMock.MockInstance;
			int recordCount = _executionState.Execute(command);

			Assert.AreEqual(1, recordCount);

			_commandMock.Verify();
			_connectionMock.Verify();
		}

		[Test]
		public void ExecuteWithAnException()
		{
			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_commandMock.ExpectAndThrow("ExecuteNonQuery", new ApplicationException("Okay"));
			_commandMock.Expect("Connection", new IsNull());
			_connectionMock.Expect("Close");

			try
			{
				_executionState.Execute((IDbCommand) _commandMock.MockInstance);
				Assert.Fail("Should have thrown the exception");
			}
			catch (ApplicationException e)
			{
				Assert.AreEqual("Okay", e.Message);
			}

			_commandMock.Verify();
			_connectionMock.Verify();
		}
	}
}
