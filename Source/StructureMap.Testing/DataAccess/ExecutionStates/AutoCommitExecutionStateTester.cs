using System;
using System.Data;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using StructureMap.DataAccess.ExecutionStates;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess.ExecutionStates
{
	[TestFixture]
	public class AutoCommitExecutionStateTester
	{
		private IMock _connectionMock;
		private IMock _commandMock;
		private AutoCommitExecutionState _executionState;
		private IMock _adapterMock;

		[SetUp]
		public void SetUp()
		{
			_connectionMock = new DynamicMock(typeof(IDbConnection));
			_commandMock = new DynamicMock(typeof(IDbCommand));
			_adapterMock = new DynamicMock(typeof(IDbDataAdapter));

			_executionState = new AutoCommitExecutionState((IDbConnection) _connectionMock.MockInstance, 
				(IDbDataAdapter) _adapterMock.MockInstance);
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

		[Test]
		public void ExecuteReaderHappyPath()
		{
			IDataReader tableReader = new TableDataReader(new DataTable());

			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_commandMock.ExpectAndReturn("ExecuteReader", tableReader, CommandBehavior.CloseConnection);

			IDbCommand command = (IDbCommand) _commandMock.MockInstance;
			IDataReader reader = _executionState.ExecuteReader(command);

			Assert.AreSame(tableReader, reader);

			_commandMock.Verify();
			_connectionMock.Verify();
		}


		[Test]
		public void ExecuteReaderWithException()
		{
			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_commandMock.ExpectAndThrow("ExecuteReader", new ApplicationException("Okay"), CommandBehavior.CloseConnection);
			_commandMock.Expect("Connection", new IsNull());
			_connectionMock.Expect("Close");

			try
			{
				_executionState.ExecuteReader((IDbCommand) _commandMock.MockInstance);
				Assert.Fail("Should have thrown the exception");
			}
			catch (ApplicationException e)
			{
				Assert.AreEqual("Okay", e.Message);
			}

			_commandMock.Verify();
			_connectionMock.Verify();			
		}

		[Test, Ignore("Problem with mocking IDbDataAdapter.Fill()")]
		public void ExecuteDataSetHappyPath()
		{
			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_commandMock.Expect("Connection", new IsNull());
			_adapterMock.Expect("Fill", new IsTypeOf(typeof(DataSet)));
			_adapterMock.Expect("SelectCommand", _commandMock.MockInstance);

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
			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_adapterMock.ExpectAndThrow("Fill", new ApplicationException("Okay"), new IsTypeOf(typeof(DataSet)));
			_adapterMock.Expect("SelectCommand", _commandMock.MockInstance);
			_commandMock.Expect("Connection", new IsNull());
			_connectionMock.Expect("Close");

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
		public void ExecuteScalarHappyPath()
		{
			object returnValue = new object();

			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_commandMock.Expect("Connection", null);
			_commandMock.ExpectAndReturn("ExecuteScalar", returnValue);
			_connectionMock.Expect("Close");

			IDbCommand command = (IDbCommand) _commandMock.MockInstance;
			object actual = _executionState.ExecuteScalar(command);

			Assert.AreSame(returnValue, actual);

			_commandMock.Verify();
			_connectionMock.Verify();
		}


		[Test]
		public void ExecuteScalarWithException()
		{
			_connectionMock.Expect("Open");
			_commandMock.Expect("Connection", _connectionMock.MockInstance);
			_commandMock.ExpectAndThrow("ExecuteScalar", new ApplicationException("Okay"));
			_commandMock.Expect("Connection", new IsNull());
			_connectionMock.Expect("Close");

			try
			{
				_executionState.ExecuteScalar((IDbCommand) _commandMock.MockInstance);
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
