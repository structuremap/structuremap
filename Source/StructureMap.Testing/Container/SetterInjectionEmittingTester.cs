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

            return new InstanceManager(pluginGraph, true);
        }

        [Test]
        public void ChildArraySetter()
        {
            InstanceManager manager = buildInstanceManager();

            WidgetArrayGridColumn column =
                (WidgetArrayGridColumn) manager.CreateInstance(typeof (IGridColumn), "WidgetArray");

            Assert.AreEqual(3, column.Widgets.Length);
        }

        [Test]
        public void ChildObjectSetter()
        {
            InstanceManager manager = buildInstanceManager();


            WidgetGridColumn column = (WidgetGridColumn) manager.CreateInstance(typeof (IGridColumn), "BlueWidget");
            Assert.IsTrue(column.Widget is ColorWidget);
        }

        [Test]
        public void EnumSetter()
        {
            PluginFamily family = new PluginFamily(typeof (IGridColumn));
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (EnumGridColumn));
            family.Plugins.Add(plugin, true);

            InstanceFactory factory = new InstanceFactory(family, true);
            InstanceMemento memento = _source.GetMemento("Enum");

            EnumGridColumn column = (EnumGridColumn) factory.GetInstance(memento);

            Assert.AreEqual(FontStyleEnum.BodyText, column.FontStyle);
        }

        [Test]
        public void PrimitiveNonStringSetter()
        {
            PluginFamily family = new PluginFamily(typeof (IGridColumn));
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (LongGridColumn));
            family.Plugins.Add(plugin, true);

            InstanceFactory factory = new InstanceFactory(family, true);
            InstanceMemento memento = _source.GetMemento("Long");
            long count = long.Parse(memento.GetProperty("Count"));

            LongGridColumn column = (LongGridColumn) factory.GetInstance(memento);


            Assert.AreEqual(count, column.Count);
        }

        [Test]
        public void StringSetter()
        {
            PluginFamily family = new PluginFamily(typeof (IGridColumn));
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (StringGridColumn));
            family.Plugins.Add(plugin, true);

            InstanceFactory factory = new InstanceFactory(family, true);
            InstanceMemento memento = _source.GetMemento("String");

            StringGridColumn column = (StringGridColumn) factory.GetInstance(memento);
            Assert.AreEqual(memento.GetProperty("Name"), column.Name);
        }
    }
}