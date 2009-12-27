using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Graph;

namespace StructureMap.Testing
{
    [TestFixture]
    public class PipelineGraphTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void can_iterate_through_empty_array_of_pipelineGraph_for_a_type_that_is_not_registered()
        {
            var graph = new PluginGraph();
            var pipeline = new PipelineGraph(graph);

            pipeline.InstancesOf(typeof (ISomething)).Count().ShouldEqual(0);
        }

        [Test]
        public void can_iterate_through_families_including_both_generics_and_normal()
        {
            var registry = new Registry();
            registry.ForRequestedType(typeof (IService<>))
                .AddConcreteType(typeof (Service<>))
                .AddConcreteType(typeof (Service2<>));

            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            PluginGraph graph = registry.Build();
            var pipeline = new PipelineGraph(graph);

            pipeline.PluginTypes.Count().ShouldEqual(2);
        }


        [Test]
        public void can_iterate_through_instance_of_pipelineGraph_for_generics()
        {
            var registry = new Registry();
            registry.ForRequestedType(typeof (IService<>))
                .AddConcreteType(typeof (Service<>))
                .AddConcreteType(typeof (Service2<>));

            PluginGraph graph = registry.Build();
            var pipeline = new PipelineGraph(graph);

            foreach (IInstance instance in pipeline.InstancesOf(typeof (IService<>)))
            {
                Debug.WriteLine(instance.Description);
            }

            pipeline.InstancesOf(typeof (IService<>)).Count().ShouldEqual(2);
        }

        [Test]
        public void can_iterate_through_instances_of_pipelineGraph_for_generics_if_not_registered()
        {
            var pipeline = new PipelineGraph(new PluginGraph());
            pipeline.InstancesOf(typeof (IService<>)).Count().ShouldEqual(0);
        }

        [Test]
        public void can_iterate_through_instances_of_pipelineGraph_on_a_type_that_is_registered()
        {
            var registry = new Registry();
            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            PluginGraph graph = registry.Build();

            var pipeline = new PipelineGraph(graph);

            pipeline.InstancesOf(typeof (ISomething)).Count().ShouldEqual(2);

            pipeline.Inject<ISomething>(new SomethingTwo());

            pipeline.InstancesOf(typeof (ISomething)).Count().ShouldEqual(3);
        }
    }
}