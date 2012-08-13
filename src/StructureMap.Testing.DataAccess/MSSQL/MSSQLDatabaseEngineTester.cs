using System;
using System.Data.SqlClient;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.MSSQL;

namespace StructureMap.Testing.DataAccess.MSSQL
{
	[TestFixture]
	public class MSSQLDatabaseEngineTester
	{
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
	}
}
