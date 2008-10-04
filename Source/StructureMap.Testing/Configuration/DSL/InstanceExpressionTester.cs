using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class InstanceExpressionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BlowUpIfNoPropertyIsFoundForType()
        {
            new SmartInstance<AWidget>().CtorDependency<Rule>();
        }
    }
}