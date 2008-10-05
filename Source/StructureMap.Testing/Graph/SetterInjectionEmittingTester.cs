using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget5;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class SetterInjectionEmittingTester
    {
        private MementoSource _source;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            DataMother.WriteDocument("GridColumnInstances.xml");
            DataMother.WriteDocument("FullTesting.XML");
            _source = new XmlFileMementoSource("GridColumnInstances.xml", "//GridColumns", "GridColumn");
        }


        private Container buildInstanceManager()
        {
            PluginGraph pluginGraph =
                DataMother.BuildPluginGraphFromXml(
                    @"
<StructureMap>
	<Assembly Name='StructureMap.Testing.Widget'/>
	<Assembly Name='StructureMap.Testing.Widget5'/>
	
	<PluginFamily Type='StructureMap.Testing.Widget.IWidget' Assembly='StructureMap.Testing.Widget' DefaultKey='Money'>
		<Source Type='XmlFile' FilePath='FullTesting.XML' XPath='Widgets' NodeName='Widget'/>
		<Plugin Assembly='StructureMap.Testing.Widget' Type='StructureMap.Testing.Widget.NotPluggableWidget' ConcreteKey='NotPluggable'/>
	</PluginFamily>

	<PluginFamily Type='StructureMap.Testing.Widget.Rule' Assembly='StructureMap.Testing.Widget' DefaultKey='Blue'>
		<Instance Key='Bigger' Type='GreaterThan'>
			<Property Name='Attribute' Value='MyDad' />
			<Property Name='Value' Value='10' />
		</Instance>
		<Instance Key='Blue' Type='Color'>
			<Property Name='color' Value='Blue' />
		</Instance>
		<Instance Key='Red' Type='Color'>
			<Property Name='color' Value='Red' />
		</Instance>
	</PluginFamily>

	<PluginFamily Type='StructureMap.Testing.Widget5.IGridColumn' Assembly='StructureMap.Testing.Widget5' DefaultKey=''>
		<Source Type='XmlFile' FilePath='GridColumnInstances.XML' XPath='//GridColumns' NodeName='GridColumn'/>
		<Plugin Assembly='StructureMap.Testing.Widget5' Type='StructureMap.Testing.Widget5.OtherGridColumn' ConcreteKey='Other'>
			<Setter Name='ColumnName' />
			<Setter Name='FontStyle' />
			<Setter Name='Rules' />
			<Setter Name='Widget' />
			<Setter Name='WrapLines' />
		</Plugin>
	</PluginFamily>
	
	<Instances/>
</StructureMap>


");


            return new Container(pluginGraph);
        }

        [Test]
        public void ChildArraySetter()
        {
            Container manager = buildInstanceManager();

            var column =
                (WidgetArrayGridColumn) manager.GetInstance(typeof (IGridColumn), "WidgetArray");

            Assert.AreEqual(3, column.Widgets.Length);
        }

        [Test]
        public void ChildObjectSetter()
        {
            Container manager = buildInstanceManager();


            var column = (WidgetGridColumn) manager.GetInstance(typeof (IGridColumn), "BlueWidget");
            Assert.IsTrue(column.Widget is ColorWidget);
        }

        [Test]
        public void EnumSetter()
        {
            var graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (IGridColumn));
            family.AddPlugin(typeof (EnumGridColumn));

            family.AddInstance(_source.GetMemento("Enum"));

            var manager = new Container(graph);

            var column = (EnumGridColumn) manager.GetInstance<IGridColumn>("Enum");

            Assert.AreEqual(FontStyleEnum.BodyText, column.FontStyle);
        }

        [Test]
        public void PrimitiveNonStringSetter()
        {
            var graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (IGridColumn));
            family.AddPlugin(typeof (LongGridColumn));

            InstanceMemento memento = _source.GetMemento("Long");
            long count = long.Parse(memento.GetProperty("Count"));
            family.AddInstance(memento);

            var manager = new Container(graph);


            var column = (LongGridColumn) manager.GetInstance<IGridColumn>("Long");
            Assert.AreEqual(count, column.Count);
        }

        [Test]
        public void StringSetter()
        {
            var graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (IGridColumn));
            family.AddPlugin(typeof (StringGridColumn));

            InstanceMemento memento = _source.GetMemento("String");
            family.AddInstance(memento);

            var manager = new Container(graph);
            var column = (StringGridColumn) manager.GetInstance<IGridColumn>("String");


            Assert.AreEqual(memento.GetProperty("Name"), column.Name);
        }
    }
}