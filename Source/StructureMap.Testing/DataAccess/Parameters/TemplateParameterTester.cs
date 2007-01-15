using System.Text;
using NUnit.Framework;
using StructureMap.DataAccess.Parameters;

namespace StructureMap.Testing.DataAccess.Parameters
{
	[TestFixture]
	public class TemplateParameterTester
	{
		[Test]
		public void SubstituteParameter()
		{
			TemplateParameter parameter = new TemplateParameter("TableName");
			parameter.SetProperty("Table1");


			StringBuilder sb = new StringBuilder("The name of the table is *{TableName}*");
			parameter.Substitute(sb);

			Assert.AreEqual("The name of the table is *Table1*", sb.ToString());
		}
	}
}
