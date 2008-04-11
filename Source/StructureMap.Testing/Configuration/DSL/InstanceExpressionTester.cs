using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
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
            Registry.Instance<IWidget>().UsingConcreteType<AWidget>().Child<Rule>();
        }
    }
}