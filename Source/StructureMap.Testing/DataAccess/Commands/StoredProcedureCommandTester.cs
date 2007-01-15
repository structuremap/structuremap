using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.Commands;

namespace StructureMap.Testing.DataAccess.Commands
{
	[TestFixture, Explicit]
	public class StoredProcedureCommandTester
	{
		private StoredProcedureCommand _command;
		public const string CREATION_SPROC_NAME = "sp_CreationTestProcedure";

		[SetUp]
		public void SetUp()
		{
			_command = new StoredProcedureCommand(CREATION_SPROC_NAME);
			IDatabaseEngine engine = ObjectMother.MSSQLDatabaseEngine();

			_command.Initialize(engine);
		}

		[Test]
		public void Initialize()
		{
			Assert.AreEqual(6, _command.Parameters.Count);
			Assert.IsNotNull(_command.Parameters["@param1"]);
			Assert.IsNotNull(_command.Parameters["@param2"]);
			Assert.IsNotNull(_command.Parameters["@param3"]);
			Assert.IsNotNull(_command.Parameters["@param4"]);
			Assert.IsNotNull(_command.Parameters["@param5"]);
		}

		/*
		CREATE PROCEDURE sp_CreationTestProcedure (
		  @param1 varchar(100),
		  @param2 VarChar(100),
		  @param3 decimal out,
		  @param4 int out,
		  @param5 varchar(100) out
		  )
		as

		  select @param5 = @param1;
		  select @param3 = 1.5;
		  select @param4 = 2;
		GO
		 */

		[Test]
		public void ExecuteAgainstDataSessionHappyPath()
		{
			_command["@param1"] = "Hello!";
			_command["@param2"] = "Goodbye.";

			DataSession session = ObjectMother.MSSQLDataSession();
			_command.Attach(session);

			_command.Execute();

			Assert.AreEqual("Hello!", (string)_command["@param5"]);
			Assert.AreEqual(2, (int)_command["@param4"]);
		}
	}
}
