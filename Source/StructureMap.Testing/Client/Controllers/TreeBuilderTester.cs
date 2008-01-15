using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Client.Controllers;
using StructureMap.Client.TreeNodes;
using StructureMap.Client.Views;
using StructureMap.Configuration;
using StructureMap.Configuration.Mementos;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;
using StructureMap.Graph;
using StructureMap.Testing.Configuration.Tokens;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Client.Controllers
{
    [TestFixture]
    public class TreeBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            ReportObjectMother mother = new ReportObjectMother();
            mother.AddFamily(typeof (IGateway));
            mother.AddPlugin(typeof (IGateway), typeof (StubbedGateway), "Stubbed");

            mother.AddPlugin(typeof (IGateway), typeof (DecoratedGateway), THE_CONCRETE_KEY);
            mother.AddChildProperty(typeof (IGateway), THE_CONCRETE_KEY, THE_PROPERTY_NAME, typeof (IGateway));

            mother.AddChildArrayProperty(typeof (IGateway), THE_CONCRETE_KEY, ARRAY_PROPERTY_NAME, typeof (IGateway));

            _report = mother.Report;
            _builder = new TreeBuilder(_report);

            _topNodeExpectation = new TreeNodeExpectation("PluginGraph", ViewConstants.SUMMARY, _report);
        }

        #endregion

        private const string THE_CONCRETE_KEY = "Decorated";
        private const string THE_PROPERTY_NAME = "innerGateway";
        private const string ARRAY_PROPERTY_NAME = "innerGatewayArray";

        private TreeNodeExpectation _topNodeExpectation;
        private PluginGraphReport _report;
        private TreeBuilder _builder;

        private ChildArrayProperty createChildArrayProperty()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            parent.AddChildArray(THE_PROPERTY_NAME, new InstanceMemento[]
                                                        {
                                                            MemoryInstanceMemento.CreateDefaultInstanceMemento(),
                                                            MemoryInstanceMemento.CreateReferencedInstanceMemento("Ref1")
                                                            ,
                                                            MemoryInstanceMemento.CreateReferencedInstanceMemento("Ref2")
                                                            ,
                                                            MemoryInstanceMemento.CreateReferencedInstanceMemento("Ref3")
                                                        });


            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            return (ChildArrayProperty) instance[THE_PROPERTY_NAME];
        }

        [Test]
        public void AsssemblyNode()
        {
            AssemblyToken assembly = new AssemblyToken("Assembly1", new string[0]);
            _builder.HandleAssembly(assembly);

            GraphObjectNode topNode = _builder.TopNode;

            _topNodeExpectation.AddChild("Assembly1", ViewConstants.ASSEMBLY, assembly);
            _topNodeExpectation.Verify(topNode);
        }

        [Test]
        public void BuildTree()
        {
            PluginGraphReport report = ObjectMother.Report();
            TreeBuilder builder = new TreeBuilder(report);

            GraphObjectNode topNode = builder.BuildTree();

            foreach (GraphObjectNode child in topNode.Nodes)
            {
                Debug.WriteLine(child.Text);
            }

            GraphObjectNode assemblies = topNode.FindChild(ViewConstants.ASSEMBLIES);
            Assert.IsNotNull(assemblies);
            Assert.AreEqual(report.Assemblies.Length, assemblies.Nodes.Count);

            GraphObjectNode families = topNode.FindChild(ViewConstants.PLUGINFAMILIES);
            Assert.IsNotNull(families);
            Assert.AreEqual(report.Families.Length, families.Nodes.Count);

            // first family node here
            // Plugins
            // Interceptors
            // instances

            foreach (GraphObjectNode familyNode in families.Nodes)
            {
                FamilyToken family = (FamilyToken) familyNode.Subject;

                GraphObjectNode interceptors = familyNode.FindChild(ViewConstants.INTERCEPTORS);
                Assert.AreEqual(family.Interceptors.Length, interceptors.Nodes.Count);

                GraphObjectNode plugins = familyNode.FindChild(ViewConstants.PLUGINS);
                Assert.AreEqual(family.Plugins.Length, plugins.Nodes.Count);

                GraphObjectNode instances = familyNode.FindChild(ViewConstants.INSTANCES);

                Debug.WriteLine("");
                Debug.WriteLine(family.PluginTypeName);
                Debug.WriteLine("Expected instances");
                foreach (InstanceToken instance in family.Instances)
                {
                    Debug.WriteLine(instance.InstanceKey);
                }

                Debug.WriteLine("Actual instances");
                foreach (GraphObjectNode child in instances.Nodes)
                {
                    Debug.WriteLine(child.Text);
                }

                Assert.AreEqual(family.Instances.Length, instances.Nodes.Count);
            }
        }

        [Test]
        public void EnumerationPropertyNode()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Property1", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Enumeration);

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "");
            memento.SetProperty(definition.PropertyName, "red");

            EnumerationProperty property = new EnumerationProperty(definition, memento);

            _builder.HandleEnumerationProperty(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(property.PropertyName, ViewConstants.ENUMERATION_PROPERTY, property);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void FamilyNode()
        {
            FamilyToken family = new FamilyToken(GetType(), "", new string[0]);
            _builder.HandleFamily(family);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(family.PluginTypeName, ViewConstants.PLUGINFAMILY, family);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void GetTheTopNode()
        {
            GraphObjectNode topNode = _builder.TopNode;
            _topNodeExpectation.Verify(topNode);
        }

        [Test]
        public void HandleChildArrayProperty()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Prop1", typeof (IGateway), PropertyDefinitionType.Constructor,
                                       ArgumentType.ChildArray);

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.AddChildArray("Prop1", new InstanceMemento[0]);

            ChildArrayProperty property = new ChildArrayProperty(definition, memento, new PluginGraphReport());

            _builder.HandleChildArrayProperty(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(property.PropertyName, ViewConstants.CHILD_ARRAY_PROPERTY, property);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void HandleDefaultChildProperty()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateDefaultInstanceMemento();
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            _builder.HandleDefaultChildProperty(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(property.PropertyName, ViewConstants.DEFAULT_CHILD_PROPERTY, property);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void HandlePropertyDefinition()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Prop1", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive);

            _builder.HandlePropertyDefinition(definition);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(definition.PropertyName, ViewConstants.PROPERTY_DEFINITION, definition);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void HandleReferenceChildProperty()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento("ref");
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            _builder.HandleReferenceChildProperty(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(property.PropertyName, ViewConstants.REFERENCE_CHILD_PROPERTY, property);
            expectation.Verify(_builder.LastNode);
        }


        [Test]
        public void HandleTemplateProperty()
        {
            TemplateProperty property = new TemplateProperty("prop1", "red");
            _builder.HandleTemplateProperty(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(property.PropertyName, ViewConstants.TEMPLATE_PROPERTY, property);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void InlineChildPropertyNode()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = new MemoryInstanceMemento("Stubbed", "inner");
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            _builder.HandleInlineChildProperty(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(property.PropertyName, ViewConstants.INLINE_CHILD_PROPERTY, property);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void InstanceNode()
        {
            InstanceToken instance = new InstanceToken();
            instance.InstanceKey = "Name";

            _builder.HandleInstance(instance);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(instance.InstanceKey, ViewConstants.INSTANCE, instance);

            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void InterceptorNode()
        {
            InterceptorInstanceToken interceptor = new InterceptorInstanceToken();
            interceptor.ConcreteKey = "concrete";
            _builder.HandleInterceptor(interceptor);

            TreeNodeExpectation expectation = new TreeNodeExpectation("concrete", ViewConstants.INSTANCE, interceptor);

            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void MementoSourceNode()
        {
            MementoSourceInstanceToken source = new MementoSourceInstanceToken();
            _builder.HandleMementoSource(source);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(ViewConstants.MEMENTO_SOURCE, ViewConstants.INSTANCE, source);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void PluginNode()
        {
            TypePath path = TypePath.TypePathForFullName("some type name");
            PluginToken plugin = new PluginToken(path, "concrete", DefinitionSource.Explicit);
            _builder.HandlePlugin(plugin);

            TreeNodeExpectation expectation = new TreeNodeExpectation("concrete", ViewConstants.PLUGIN, plugin);

            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void PrimitivePropertyNode()
        {
            PropertyDefinition definition =
                new PropertyDefinition("Property1", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive);

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "");
            memento.SetProperty(definition.PropertyName, "red");

            PrimitiveProperty property = new PrimitiveProperty(definition, memento);

            _builder.HandlePrimitiveProperty(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation(property.PropertyName, ViewConstants.PRIMITIVE_PROPERTY, property);
            expectation.Verify(_builder.LastNode);
        }

        [Test]
        public void PropertyDefinitionNode()
        {
            PropertyDefinition property =
                new PropertyDefinition("property1", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive);
            _builder.HandlePropertyDefinition(property);

            TreeNodeExpectation expectation =
                new TreeNodeExpectation("property1", ViewConstants.PROPERTY_DEFINITION, property);

            expectation.Verify(_builder.LastNode);
        }
    }
}