using System;
using System.Data.SqlClient;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.MSSQL;
using StructureMap.DataAccess.Parameterization;
using StructureMap.DataAccess.Parameters;

namespace StructureMap.Testing.DataAccess.Parameterization
{
    [TestFixture]
    public class ParameterizedCommandBuilderTester
    {
        [Test]
        public void OneParameterNoMatches()
        {
            MSSQLDatabaseEngine engine = ObjectMother.MSSQLDatabaseEngine();
            string template = "select * from table where column1 = {flag1}";

            ParameterizedCommandBuilder builder = new ParameterizedCommandBuilder(engine, template);
            builder.Build();
            SqlCommand command = (SqlCommand) builder.Command;

            string expectedCommandText = "select * from table where column1 = @flag1";
            Assert.AreEqual(expectedCommandText, command.CommandText);

            Assert.AreEqual(1, command.Parameters.Count);

            SqlParameter parameter = command.Parameters[0];
            Assert.AreEqual("@flag1", parameter.ParameterName);

            parameter.Value = DBNull.Value;
            Assert.AreEqual(DBNull.Value, parameter.Value);

            ParameterCollection parameters = builder.Parameters;
            Assert.AreEqual(1, parameters.Count);
            Assert.AreSame(parameter, ((Parameter) parameters["flag1"]).InnerParameter);
        }


        [Test]
        public void ParameterizedCommandCanBeNulled()
        {
            ParameterizedCommand command = new ParameterizedCommand("select * from table1 where col1 = {a}");
            command.Initialize(new MSSQLDatabaseEngine("something"));

            command["a"] = DBNull.Value;
            Assert.AreEqual(command["a"], DBNull.Value);
        }
    }
}