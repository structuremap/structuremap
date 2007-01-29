using System;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration.Tokens
{
    [TestFixture]
    public class PluginTokenTester
    {
        [Test]
        public void ReadProperties()
        {
            Plugin plugin = Plugin.CreateExplicitPlugin(typeof (PluginTokenTarget), "concrete", string.Empty);

            PluginToken token =
                new PluginToken(new TypePath(typeof (PluginToken)), "concrete", DefinitionSource.Explicit);
            token.ReadProperties(plugin);

            PropertyDefinition[] expected = new PropertyDefinition[]
                {
                    new PropertyDefinition("Name", typeof (string), PropertyDefinitionType.Setter,
                                           ArgumentType.Primitive),
                    new PropertyDefinition("active", typeof (bool), PropertyDefinitionType.Constructor,
                                           ArgumentType.Primitive)
                };

            Assert.AreEqual(expected, token.Properties);
        }

        [Test]
        public void CreateImplicitToken()
        {
            Type pluggedType = typeof (ComplexRule);
            Plugin plugin = Plugin.CreateImplicitPlugin(pluggedType);
            string expectedConcreteKey = "Complex";

            PluginToken token = PluginToken.CreateImplicitToken(plugin);

            PluginToken expected =
                new PluginToken(new TypePath(pluggedType), expectedConcreteKey, DefinitionSource.Implicit);
            Assert.AreEqual(expected, token);

            Assert.AreEqual(plugin.GetConstructor().GetParameters().Length, token.Properties.Length);
        }
    }


    public class PluginTokenTarget
    {
        private string _name;
        private int _age;
        private bool _active;

        public PluginTokenTarget(bool active)
        {
            _active = active;
        }

        [SetterProperty]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
    }
}