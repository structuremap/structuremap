using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.MSSQL;

namespace StructureMap.Testing.DataAccess.MSSQL
{
	[TestFixture, Explicit]
	public class MSSQLDatabaseEngineTester
	{
		public const string CREATION_SPROC_NAME = "sp_CreationTestProcedure";

		[Test]
		public void CreateConnection()
		{
			string theConnectionString = "Data Source=localhost;database=test";

			IDatabaseEngine database = new MSSQLDatabaseEngine(theConnectionString);
		
			SqlConnection connection = (SqlConnection) database.GetConnection();
			Assert.AreEqual(theConnectionString, connection.ConnectionString);
		}

		[Test]
		public void CreateCommand()
		{
			string theConnectionString = "Data Source=localhost;database=test";

			IDatabaseEngine database = new MSSQLDatabaseEngine(theConnectionString);
			SqlCommand command = (SqlCommand) database.GetCommand();
		}

		[Test]
		public void CreateDataAdapter()
		{
			string theConnectionString = "Data Source=localhost;database=test";
			IDatabaseEngine database = new MSSQLDatabaseEngine(theConnectionString);
			SqlDataAdapter adapter = (SqlDataAdapter) database.GetDataAdapter();
		
			Assert.IsNotNull(adapter);
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
		public void CreateStoredProcedureCommand()
		{
			MSSQLDatabaseEngine engine = ObjectMother.MSSQLDatabaseEngine();

			SqlCommand command = (SqlCommand) engine.CreateStoredProcedureCommand(CREATION_SPROC_NAME);

			Assert.AreEqual(CREATION_SPROC_NAME, command.CommandText);
			Assert.AreEqual(CommandType.StoredProcedure, command.CommandType);

			Assert.AreEqual(6, command.Parameters.Count, "5 parameters + RETURN_VALUE");

			Assert.IsTrue(command.Parameters.Contains("@param1"));
			Assert.IsTrue(command.Parameters.Contains("@param2"));
			Assert.IsTrue(command.Parameters.Contains("@param3"));
			Assert.IsTrue(command.Parameters.Contains("@param4"));
			Assert.IsTrue(command.Parameters.Contains("@param5"));
		}

		[Test]
		public void CreateStringParameter()
		{
			MSSQLDatabaseEngine engine = ObjectMother.MSSQLDatabaseEngine();

			SqlParameter parameter = (SqlParameter) engine.CreateStringParameter("param1", 30, true);

			Assert.AreEqual( "@param1", parameter.ParameterName);
			Assert.AreEqual(30, parameter.Size);
			Assert.AreEqual(true, parameter.IsNullable);
			Assert.AreEqual(DbType.String, parameter.DbType);
		}

		[Test]
		public void CreateParameter()
		{
			MSSQLDatabaseEngine engine = ObjectMother.MSSQLDatabaseEngine();
			SqlParameter parameter = (SqlParameter) engine.CreateParameter("param2", DbType.Int32, false);

			Assert.AreEqual("@param2", parameter.ParameterName);
			Assert.AreEqual(DbType.Int32, parameter.DbType);
			Assert.AreEqual(false, parameter.IsNullable);
		}
	}
}
