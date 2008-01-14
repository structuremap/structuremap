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
        [SetUp]
        public void SetUp()
        {
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  301\nNo concrete type or concrete key is specified for instance TheInstanceKey for PluginType StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget"
             )]
        public void BlowUpIfNoConcreteKeyOrTypeDefinied()
        {
            InstanceExpression expression = new InstanceExpression(typeof (IWidget));
            expression.InstanceKey = "TheInstanceKey";
            PluginGraph pluginGraph = new PluginGraph();
            ((IExpression) expression).Configure(pluginGraph);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BlowUpIfNoPropertyIsFoundForType()
        {
            Registry.Instance<IWidget>().UsingConcreteType<AWidget>().Child<Rule>();
        }
    }
}