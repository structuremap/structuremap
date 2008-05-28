using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget5;

namespace StructureMap.Testing.Container
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


        private InstanceManager buildInstanceManager()
        {
            PluginGraph pluginGraph = DataMother.GetDiagnosticPluginGraph("SetterInjectionTesting.xml");

            return new InstanceManager(pluginGraph);
        }

        [Test]
        public void ChildArraySetter()
        {
            InstanceManager manager = buildInstanceManager();

            WidgetArrayGridColumn column =
                (WidgetArrayGridColumn) manager.GetInstance(typeof (IGridColumn), "WidgetArray");

            Assert.AreEqual(3, column.Widgets.Length);
        }

        [Test]
        public void ChildObjectSetter()
        {
            InstanceManager manager = buildInstanceManager();


            WidgetGridColumn column = (WidgetGridColumn) manager.GetInstance(typeof (IGridColumn), "BlueWidget");
            Assert.IsTrue(column.Widget is ColorWidget);
        }

        [Test]
        public void EnumSetter()
        {
            PluginGraph graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (IGridColumn));
            Plugin plugin = new Plugin(typeof(EnumGridColumn));
            family.AddPlugin(plugin);

            family.AddInstance(_source.GetMemento("Enum"));

            InstanceManager manager = new InstanceManager(graph);

            EnumGridColumn column = (EnumGridColumn) manager.GetInstance<IGridColumn>("Enum");

            Assert.AreEqual(FontStyleEnum.BodyText, column.FontStyle);
        }

        [Test]
        public void PrimitiveNonStringSetter()
        {
            PluginGraph graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof(IGridColumn));
            Plugin plugin = new Plugin(typeof(LongGridColumn));
            family.AddPlugin(plugin);

            InstanceMemento memento = _source.GetMemento("Long");
            long count = long.Parse(memento.GetProperty("Count"));
            family.AddInstance(memento);

            InstanceManager manager = new InstanceManager(graph);


            LongGridColumn column = (LongGridColumn) manager.GetInstance<IGridColumn>("Long");
            Assert.AreEqual(count, column.Count);
        }

        [Test]
        public void StringSetter()
        {
            PluginGraph graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof(IGridColumn));
            Plugin plugin = new Plugin(typeof(StringGridColumn));
            family.AddPlugin(plugin);

            InstanceMemento memento = _source.GetMemento("String");
            family.AddInstance(memento);

            InstanceManager manager = new InstanceManager(graph);
            StringGridColumn column = (StringGridColumn) manager.GetInstance<IGridColumn>("String");



            Assert.AreEqual(memento.GetProperty("Name"), column.Name);
        }
    }
}