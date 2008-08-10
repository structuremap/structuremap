using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class InstanceMementoInstanceCreationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _graph = new PluginGraph();
            PluginFamily family = _graph.FindFamily(typeof (IService));
            family.AddPlugin(typeof (ColorService), "Color");

            _graph.FindFamily(typeof (Rule));
        }

        #endregion

        private PluginGraph _graph;

        public class Rule
        {
        }

        public class ComplexRule : Rule
        {
            private readonly bool _Bool;
            private readonly byte _Byte;
            private readonly BreedEnum _color;
            private readonly double _Double;
            private readonly int _Int;
            private readonly long _Long;
            private readonly string _String;


            [DefaultConstructor]
            public ComplexRule(string String, BreedEnum Color, int Int, long Long, byte Byte, double Double, bool Bool,
                               IAutomobile car, IAutomobile[] cars)
            {
                _String = String;
                _color = Color;
                _Int = Int;
                _Long = Long;
                _Byte = Byte;
                _Double = Double;
                _Bool = Bool;
            }

            /// <summary>
            /// Plugin should find the constructor above, not the "greedy" one below.
            /// </summary>
            /// <param name="String"></param>
            /// <param name="String2"></param>
            /// <param name="Int"></param>
            /// <param name="Long"></param>
            /// <param name="Byte"></param>
            /// <param name="Double"></param>
            /// <param name="Bool"></param>
            /// <param name="extra"></param>
            public ComplexRule(string String, string String2, int Int, long Long, byte Byte, double Double, bool Bool,
                               string extra)
            {
            }

            public string String
            {
                get { return _String; }
            }


            public int Int
            {
                get { return _Int; }
            }

            public byte Byte
            {
                get { return _Byte; }
            }

            public long Long
            {
                get { return _Long; }
            }

            public double Double
            {
                get { return _Double; }
            }

            public bool Bool
            {
                get { return _Bool; }
            }

            public static MemoryInstanceMemento GetMemento()
            {
                MemoryInstanceMemento memento = new MemoryInstanceMemento("", "Sample");
                memento.SetProperty("String", "Red");
                memento.SetProperty("Color", "Green");
                memento.SetProperty("Int", "1");
                memento.SetProperty("Long", "2");
                memento.SetProperty("Byte", "3");
                memento.SetProperty("Double", "4");
                memento.SetProperty("Bool", "true");

                return memento;
            }
        }


        public interface IAutomobile
        {
        }

        public class GrandPrix : IAutomobile
        {
            private readonly string _color;
            private readonly int _horsePower;

            public GrandPrix(int horsePower, string color)
            {
                _horsePower = horsePower;
                _color = color;
            }
        }

        public class Mustang : IAutomobile
        {
        }

        private void assertIsReference(Instance instance, string referenceKey)
        {
            ReferencedInstance referencedInstance = (ReferencedInstance) instance;
            Assert.AreEqual(referenceKey, referencedInstance.ReferenceKey);
        }

        [Test]
        public void Create_a_default_instance()
        {
            MemoryInstanceMemento memento = MemoryInstanceMemento.CreateDefaultInstanceMemento();
            Instance instance = memento.ReadInstance(null, null);

            Assert.IsInstanceOfType(typeof (DefaultInstance), instance);
        }

        [Test]
        public void Create_a_referenced_instance()
        {
            MemoryInstanceMemento memento = MemoryInstanceMemento.CreateReferencedInstanceMemento("blue");
            ReferencedInstance instance = (ReferencedInstance) memento.ReadInstance(null, null);

            Assert.AreEqual("blue", instance.ReferenceKey);
        }


        [Test]
        public void Get_the_instance_name()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("Color", "Red");
            memento.SetProperty("Color", "Red");
            memento.InstanceKey = "Red";

            Assert.AreEqual("Red", memento.ReadInstance(_graph, typeof (IService)).Name);
        }

        [Test]
        public void ReadChildArrayProperty()
        {
            PluginGraph graph = new PluginGraph();

            graph.FindFamily(typeof(Rule)).AddPlugin(typeof(ComplexRule));

            MemoryInstanceMemento memento = ComplexRule.GetMemento();
            memento.SetProperty(XmlConstants.PLUGGED_TYPE, typeof (ComplexRule).AssemblyQualifiedName);
            memento.AddChildArray("cars", new InstanceMemento[]
                                              {
                                                  MemoryInstanceMemento.CreateReferencedInstanceMemento("Ford"),
                                                  MemoryInstanceMemento.CreateReferencedInstanceMemento("Chevy"),
                                                  MemoryInstanceMemento.CreateReferencedInstanceMemento("Dodge"),
                                              });

            IStructuredInstance instance = (IStructuredInstance) memento.ReadInstance(graph, typeof (Rule));
            Instance[] instances = instance.GetChildArray("cars");
            Assert.AreEqual(3, instances.Length);

            assertIsReference(instances[0], "Ford");
            assertIsReference(instances[1], "Chevy");
            assertIsReference(instances[2], "Dodge");
        }

        [Test]
        public void ReadChildProperty_child_property_is_defined_build_child()
        {
            PluginGraph graph = new PluginGraph();

            graph.FindFamily(typeof(Rule)).AddPlugin(typeof(ComplexRule));

            MemoryInstanceMemento memento = ComplexRule.GetMemento();
            memento.SetProperty(XmlConstants.PLUGGED_TYPE, typeof (ComplexRule).AssemblyQualifiedName);
            MemoryInstanceMemento carMemento = MemoryInstanceMemento.CreateReferencedInstanceMemento("GrandPrix");
            memento.AddChild("car", carMemento);

            IStructuredInstance instance = (IStructuredInstance) memento.ReadInstance(graph, typeof (Rule));
            ReferencedInstance child = (ReferencedInstance) instance.GetChild("car");

            Assert.AreEqual("GrandPrix", child.ReferenceKey);
        }

        [Test]
        public void ReadPrimitivePropertiesHappyPath()
        {
            PluginGraph graph = new PluginGraph();

            graph.FindFamily(typeof(Rule)).AddPlugin(typeof(ComplexRule));

            MemoryInstanceMemento memento = ComplexRule.GetMemento();
            memento.SetProperty(XmlConstants.PLUGGED_TYPE, typeof (ComplexRule).AssemblyQualifiedName);

            IConfiguredInstance instance = (IConfiguredInstance) memento.ReadInstance(graph, typeof (Rule));

            Assert.AreEqual(memento.GetProperty("String"), instance.GetProperty("String"));
            Assert.AreEqual(memento.GetProperty("Color"), instance.GetProperty("Color"));
            Assert.AreEqual(memento.GetProperty("Int"), instance.GetProperty("Int"));
            Assert.AreEqual(memento.GetProperty("Long"), instance.GetProperty("Long"));
            Assert.AreEqual(memento.GetProperty("Byte"), instance.GetProperty("Byte"));
            Assert.AreEqual(memento.GetProperty("Double"), instance.GetProperty("Double"));
            Assert.AreEqual(memento.GetProperty("Bool"), instance.GetProperty("Bool"));
        }
    }
}