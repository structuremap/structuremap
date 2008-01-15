using System.Data;
using System.Data.SqlClient;
using NMock;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.Parameterization;

namespace StructureMap.Testing.DataAccess.Parameterization
{
    [TestFixture]
    public class ParameterTemplateTester
    {
        [Test]
        public void BasicParameterTemplate()
        {
            SqlParameter parameter = new SqlParameter();
            bool isNullable = true;
            string theParameterName = "red";
            DbType theDbType = DbType.Int32;
            BasicParameterTemplate template = new BasicParameterTemplate(theParameterName, theDbType, isNullable);

            IMock engineMock = new DynamicMock(typeof (IDatabaseEngine));
            engineMock.ExpectAndReturn("CreateParameter", parameter, theParameterName, theDbType, isNullable);

            SqlParameter parameter2 =
                (SqlParameter) template.ConfigureParameter((IDatabaseEngine) engineMock.MockInstance);

            Assert.AreSame(parameter, parameter2);
        }

        [Test]
        public void StringParameterTemplateConfigureTemplate()
        {
            SqlParameter parameter = new SqlParameter();
            int theSize = 25;
            bool isNullable = true;
            string theParameterName = "red";
            StringParameterTemplate template = new StringParameterTemplate(theParameterName, theSize, isNullable);

            IMock engineMock = new DynamicMock(typeof (IDatabaseEngine));
            engineMock.ExpectAndReturn("CreateStringParameter", parameter, theParameterName, theSize, isNullable);

            SqlParameter parameter2 =
                (SqlParameter) template.ConfigureParameter((IDatabaseEngine) engineMock.MockInstance);

            Assert.AreSame(parameter, parameter2);
        }
    }
}