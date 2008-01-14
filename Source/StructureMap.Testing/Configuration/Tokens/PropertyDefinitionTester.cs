using System;
using System.Data;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Mementos;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Testing.Configuration.Tokens
{
    [TestFixture]
    public class PropertyDefinitionTester
    {
        private Type _type;
        private ConstructorInfo _ctor;

        [SetUp]
        public void SetUp()
        {
            _type = typeof (DefinitionTarget);
            _ctor = _type.GetConstructors()[0];
        }

        [Test]
        public void BuildPropertyDefinitionsFromConstructor()
        {
            PropertyDefinition[] expected = new PropertyDefinition[]
                {
                    new PropertyDefinition("name", typeof (string), PropertyDefinitionType.Constructor,
                                           ArgumentType.Primitive),
                    new PropertyDefinition("color", typeof (Color), PropertyDefinitionType.Constructor,
                                           ArgumentType.Enumeration),
                    new PropertyDefinition("connection", typeof (IDbConnection), PropertyDefinitionType.Constructor,
                                           ArgumentType.Child),
                    new PropertyDefinition("documents", typeof (XmlDocument), PropertyDefinitionType.Constructor,
                                           ArgumentType.ChildArray)
                };

            PropertyDefinition[] actual = PropertyDefinitionBuilder.CreatePropertyDefinitions(_ctor);

            Assert.AreEqual(expected, actual);
        }

        private void validateSetter(PropertyDefinition expected, string propertyName)
        {
            PropertyInfo property = _type.GetProperty(propertyName);
            PropertyDefinition actual = PropertyDefinitionBuilder.CreatePropertyDefinition(property);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrimitiveSetter()
        {
            PropertyDefinition expected =
                new PropertyDefinition("Name", typeof (string), PropertyDefinitionType.Setter, ArgumentType.Primitive);
            validateSetter(expected, "Name");
        }


        [Test]
        public void EnumerationSetter()
        {
            PropertyDefinition expected =
                new PropertyDefinition("Color", typeof (Color), PropertyDefinitionType.Setter, ArgumentType.Enumeration);
            validateSetter(expected, "Color");
        }


        [Test]
        public void ChildSetter()
        {
            PropertyDefinition expected =
                new PropertyDefinition("Connection", typeof (IDbConnection), PropertyDefinitionType.Setter,
                                       ArgumentType.Child);
            validateSetter(expected, "Connection");
        }


        [Test]
        public void ChildArraySetter()
        {
            PropertyDefinition expected =
                new PropertyDefinition("Documents", typeof (XmlDocument), PropertyDefinitionType.Setter,
                                       ArgumentType.ChildArray);
            validateSetter(expected, "Documents");
        }

        [Test]
        public void BuildPrimitiveProperty()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Color", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive);
            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.SetProperty("Color", "red");

            PrimitiveProperty property = (PrimitiveProperty) definition.CreateProperty(memento, null);

            Assert.AreEqual("red", property.Value);
            Assert.AreEqual(definition, property.Definition);
        }

        [Test]
        public void BuildEnumerationProperty()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Color", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Enumeration);
            definition.EnumerationValues = new string[] {"red", "green", "blue"};

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.SetProperty("Color", "red");

            EnumerationProperty property = (EnumerationProperty) definition.CreateProperty(memento, null);

            Assert.AreEqual("red", property.Value);
            Assert.AreEqual(definition, property.Definition);
        }

        [Test]
        public void BuildChildProperty()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Color", typeof (string), PropertyDefinitionType.Constructor, ArgumentType.Child);

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.AddChild("Color", MemoryInstanceMemento.CreateDefaultInstanceMemento());

            PluginGraphReport report = new PluginGraphReport();

            ChildProperty property = (ChildProperty) definition.CreateProperty(memento, report);

            Assert.AreEqual(definition, property.Definition);
        }

        [Test]
        public void BuildChildArrayProperty()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Color", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.ChildArray);

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.AddChildArray("Color", new InstanceMemento[0]);

            PluginGraphReport report = new PluginGraphReport();

            ChildArrayProperty property = (ChildArrayProperty) definition.CreateProperty(memento, report);

            Assert.AreEqual(definition, property.Definition);
        }
    }

    public enum Color
    {
        Red,
        Green,
        Yellow
    }

    public class DefinitionTarget
    {
        private string _name;
        private Color _color;
        private IDbConnection _connection;
        private XmlDocument[] _documents;

        public DefinitionTarget(string name, Color color, IDbConnection connection, XmlDocument[] documents)
        {
            _name = name;
            _color = color;
            _connection = connection;
            _documents = documents;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public IDbConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public XmlDocument[] Documents
        {
            get { return _documents; }
            set { _documents = value; }
        }
    }
}