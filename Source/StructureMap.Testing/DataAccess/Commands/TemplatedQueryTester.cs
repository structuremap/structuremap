using System;
using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess.Commands;
using StructureMap.DataAccess.MSSQL;

namespace StructureMap.Testing.DataAccess.Commands
{
	[TestFixture]
	public class TemplatedQueryTester
	{
		private TemplatedQuery _query;

		[SetUp]
		public void SetUp()
		{
			_query = new TemplatedQuery("select Count({Column}) from {Table}",
				new IQueryFilter[] {
					new TemplatedQueryFilter("Name", "name = '{Value}'"),
					new TemplatedQueryFilter("State", "state = '{Value}'"),
					new ParameterizedQueryFilter("Direction", "direction = {Value}"),
								   });
				
			MSSQLDatabaseEngine engine = ObjectMother.MSSQLDatabaseEngine();
			_query.Initialize(engine);
		}

		[Test]
		public void CorrectNumberOfParameters()
		{
			Assert.AreEqual(5, _query.Parameters.Count);
		}

		[Test]
		public void JustSetTheTemplatesInTheMainBody()
		{
			_query["Column"] = "state";
			_query["Table"] = "bigtable";

			IDbCommand command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from bigtable", command.CommandText);
			Assert.AreEqual(0, command.Parameters.Count);

			_query["Table"] = "smalltable";

			command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from smalltable", command.CommandText);
			Assert.AreEqual(0, command.Parameters.Count);
		}

		[Test]
		public void OneTemplate()
		{
			_query["Column"] = "state";
			_query["Table"] = "bigtable";
			_query["Name"] = "Jeremy";

			IDbCommand command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from bigtable where name = 'Jeremy'", command.CommandText);
			Assert.AreEqual(0, command.Parameters.Count);
			
		}


		[Test]
		public void TwoTemplates()
		{
			_query["Column"] = "state";
			_query["Table"] = "bigtable";
			_query["Name"] = "Jeremy";
			_query["State"] = "Missouri";

			IDbCommand command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from bigtable where name = 'Jeremy' and state = 'Missouri'", command.CommandText);
			Assert.AreEqual(0, command.Parameters.Count);
		}

		[Test]
		public void TwoTemplatesOneParameter()
		{
			_query["Column"] = "state";
			_query["Table"] = "bigtable";
			_query["Name"] = "Jeremy";
			_query["State"] = "Missouri";
			_query["Direction"] = "North";

			IDbCommand command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from bigtable where name = 'Jeremy' and state = 'Missouri' and direction = @Direction", command.CommandText);
			Assert.AreEqual(1, command.Parameters.Count);
		}

		[Test]
		public void CreateWithParameterThenSetValueToNullNoParameters()
		{
			_query["Column"] = "state";
			_query["Table"] = "bigtable";
			_query["Direction"] = "North";

			IDbCommand command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from bigtable where direction = @Direction", command.CommandText);
			Assert.AreEqual(1, command.Parameters.Count);

			command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from bigtable where direction = @Direction", command.CommandText);
			Assert.AreEqual(1, command.Parameters.Count);

			_query["Direction"] = null;

			command = _query.ConfigureInnerCommand();

			Assert.AreEqual("select Count(state) from bigtable", command.CommandText);
			Assert.AreEqual(0, command.Parameters.Count);
		}
		
	
	}
}
