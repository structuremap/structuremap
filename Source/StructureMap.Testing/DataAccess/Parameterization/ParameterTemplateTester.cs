using NUnit.Framework;

namespace StructureMap.Testing.DataAccess.Parameterization
{
    // TODO : Rhinoize
    [TestFixture]
    public class ParameterTemplateTester
    {
        //[Test]
        //public void BasicParameterTemplate()
        //{
        //    SqlParameter parameter = new SqlParameter();
        //    bool isNullable = true;
        //    string theParameterName = "red";
        //    DbType theDbType = DbType.Int32;
        //    BasicParameterTemplate template = new BasicParameterTemplate(theParameterName, theDbType, isNullable);

        //    IMock engineMock = new DynamicMock(typeof (IDatabaseEngine));
        //    engineMock.ExpectAndReturn("CreateParameter", parameter, theParameterName, theDbType, isNullable);

        //    SqlParameter parameter2 =
        //        (SqlParameter) template.ConfigureParameter((IDatabaseEngine) engineMock.MockInstance);

        //    Assert.AreSame(parameter, parameter2);
        //}

        //[Test]
        //public void StringParameterTemplateConfigureTemplate()
        //{
        //    SqlParameter parameter = new SqlParameter();
        //    int theSize = 25;
        //    bool isNullable = true;
        //    string theParameterName = "red";
        //    StringParameterTemplate template = new StringParameterTemplate(theParameterName, theSize, isNullable);

        //    IMock engineMock = new DynamicMock(typeof (IDatabaseEngine));
        //    engineMock.ExpectAndReturn("CreateStringParameter", parameter, theParameterName, theSize, isNullable);

        //    SqlParameter parameter2 =
        //        (SqlParameter) template.ConfigureParameter((IDatabaseEngine) engineMock.MockInstance);

        //    Assert.AreSame(parameter, parameter2);
        //}
    }
}