using NMock;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Testing.Configuration.Tokens
{
	[TestFixture]
	public class TemplateTokenTester
	{
		[Test]
		public void TemplateTokenCallsCorrectMethodOnIVisitor()
		{
			IMock visitorMock = new DynamicMock(typeof(IConfigurationVisitor));
			
			TemplateToken template = new TemplateToken();

			visitorMock.Expect("HandleTemplate", template);

			template.AcceptVisitor((IConfigurationVisitor)visitorMock.MockInstance);
			visitorMock.Verify();
		}
	}
}
