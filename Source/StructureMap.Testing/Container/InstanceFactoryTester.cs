using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class InstanceFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            PluginGraph graph = new PluginGraph();
            Registry registry = new Registry(graph);
            registry.BuildInstancesOf<Rule>().AliasConcreteType<ComplexRule>("Complex");
            registry.ScanAssemblies()
                .IncludeAssembly("StructureMap.Testing.Widget")
                .IncludeAssembly("StructureMap.Testing.Widget2");

            registry.Build();

            PipelineGraph pipelineGraph = new PipelineGraph(graph);
            _session = new BuildSession(pipelineGraph, graph.InterceptorLibrary);

            _manager = registry.BuildInstanceManager();

            
        }

        #endregion

        private IInstanceManager _manager;
        private BuildSession _session;


        [Test]
        public void BuildRule1()
        {
            // TODO: Move to ConfiguredInstanceTester
            ConfiguredInstance instance = new ConfiguredInstance().WithConcreteKey("Rule1");

            Rule rule = (Rule) instance.Build(typeof(Rule), _session);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is Rule1);
        }


        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithABadValue()
        {
            // TODO -- move to ConfiguredInstanceTester
            
            ConfiguredInstance instance = (ConfiguredInstance) ComplexRule.GetInstance();

            instance.SetProperty("Int", "abc");
            ComplexRule rule = (ComplexRule)instance.Build(typeof(Rule), _session);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithAMissingValue()
        {
            // TODO:  Move to ConfiguredInstanceTester

            ConfiguredInstance instance = (ConfiguredInstance) ComplexRule.GetInstance();
            instance.RemoveKey("String");

            ComplexRule rule = (ComplexRule)instance.Build(typeof(Rule), _session);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetInstanceWithInvalidInstanceKey()
        {
            _manager.CreateInstance<Rule>("NonExistentRule");
        }

        [Test]
        public void CanMakeAClassWithNoConstructorParametersWithoutADefinedMemento()
        {
            Registry registry = new Registry();
            registry.ScanAssemblies().IncludeAssembly("StructureMap.Testing.Widget3");
            registry.BuildInstancesOf<IGateway>();

            PluginGraph graph = registry.Build();
            PipelineGraph pipelineGraph = new PipelineGraph(graph);

            IInstanceFactory factory = pipelineGraph.ForType(typeof (IGateway));

            DefaultGateway gateway = factory.Build(_session, "Default") as DefaultGateway;
            Assert.IsNotNull(gateway);
        }


        [Test]
        public void TestComplexRule()
        {
            // TODO:  Move to ConfiguredInstanceTester

            ConfiguredInstance instance = (ConfiguredInstance) ComplexRule.GetInstance();

            Rule rule = (Rule) instance.Build(typeof(Rule), _session);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is ComplexRule);
        }


    }
}