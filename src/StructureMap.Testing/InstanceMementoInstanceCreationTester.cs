using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Configuration;
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
            public ComplexRule(string String, BreedEnum Breed, int Int, long Long, byte Byte, double Double, bool Bool,
                               IAutomobile car, IAutomobile[] cars)
            {
                _String = String;
                _color = Breed;
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

            public string String { get { return _String; } }


            public int Int { get { return _Int; } }

            public byte Byte { get { return _Byte; } }

            public long Long { get { return _Long; } }

            public double Double { get { return _Double; } }

            public bool Bool { get { return _Bool; } }

            public static MemoryInstanceMemento GetMemento()
            {
                var memento = new MemoryInstanceMemento("", "Sample");
                memento.SetProperty("String", "Red");
                memento.SetProperty("Breed", "Longhorn");
                memento.SetProperty("Int", "1");
                memento.SetProperty("Long", "2");
                memento.SetProperty("Byte", "3");
                memento.SetProperty("Double", "4");
                memento.SetProperty("Bool", "True");

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
            var referencedInstance = (ReferencedInstance) instance;
            Assert.AreEqual(referenceKey, referencedInstance.ReferenceKey);
        }

        [Test]
        public void Create_a_default_instance()
        {
            MemoryInstanceMemento memento = MemoryInstanceMemento.CreateDefaultInstanceMemento();
            Instance instance = memento.ReadInstance(null, GetType());

            instance.ShouldBeOfType<DefaultInstance>();
        }

        [Test]
        public void Create_a_referenced_instance()
        {
            MemoryInstanceMemento memento = MemoryInstanceMemento.CreateReferencedInstanceMemento("blue");
            var instance = (ReferencedInstance) memento.ReadInstance(null, GetType());

            Assert.AreEqual("blue", instance.ReferenceKey);
        }


        [Test]
        public void Get_the_instance_name()
        {
            var memento = new MemoryInstanceMemento("Color", "Red");
            memento.SetPluggedType<ColorService>();
            memento.SetProperty("Color", "Red");
            memento.InstanceKey = "Red";

            Assert.AreEqual("Red", memento.ReadInstance(new SimplePluginFactory(), typeof(IService)).Name);
        }

        [Test]
        public void ReadChildArrayProperty()
        {
            MemoryInstanceMemento memento = ComplexRule.GetMemento();
            memento.SetProperty(XmlConstants.PLUGGED_TYPE, typeof (ComplexRule).AssemblyQualifiedName);
            memento.AddChildArray("cars", new InstanceMemento[]
            {
                MemoryInstanceMemento.CreateReferencedInstanceMemento("Ford"),
                MemoryInstanceMemento.CreateReferencedInstanceMemento("Chevy"),
                MemoryInstanceMemento.CreateReferencedInstanceMemento("Dodge"),
            });

            var instance = (IStructuredInstance)memento.ReadInstance(new SimplePluginFactory(), typeof(Rule));
            Instance[] instances = instance.GetChildArray("cars");
            Assert.AreEqual(3, instances.Length);

            assertIsReference(instances[0], "Ford");
            assertIsReference(instances[1], "Chevy");
            assertIsReference(instances[2], "Dodge");
        }

        [Test]
        public void ReadChildProperty_child_property_is_defined_build_child()
        {
            MemoryInstanceMemento memento = ComplexRule.GetMemento();
            memento.SetProperty(XmlConstants.PLUGGED_TYPE, typeof (ComplexRule).AssemblyQualifiedName);
            MemoryInstanceMemento carMemento = MemoryInstanceMemento.CreateReferencedInstanceMemento("GrandPrix");
            memento.AddChild("car", carMemento);

            var instance = (IStructuredInstance)memento.ReadInstance(new SimplePluginFactory(), typeof(Rule));
            var child = (ReferencedInstance) instance.GetChild("car");

            Assert.AreEqual("GrandPrix", child.ReferenceKey);
        }

        [Test]
        public void ReadPrimitivePropertiesHappyPath()
        {
            MemoryInstanceMemento memento = ComplexRule.GetMemento();
            memento.SetProperty(XmlConstants.PLUGGED_TYPE, typeof (ComplexRule).AssemblyQualifiedName);

            var instance = (IConfiguredInstance)memento.ReadInstance(new SimplePluginFactory(), typeof(Rule));


            Assert.AreEqual(memento.GetProperty("String"), instance.GetProperty("String"));
            Assert.AreEqual(memento.GetProperty("Breed"), instance.GetProperty("Breed"));
            Assert.AreEqual(memento.GetProperty("Int"), instance.GetProperty("Int"));
            Assert.AreEqual(memento.GetProperty("Long"), instance.GetProperty("Long"));
            Assert.AreEqual(memento.GetProperty("Byte"), instance.GetProperty("Byte"));
            Assert.AreEqual(memento.GetProperty("Double"), instance.GetProperty("Double"));
            Assert.AreEqual(memento.GetProperty("Bool"), instance.GetProperty("Bool"));
        }
    }
}