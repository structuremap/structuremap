using System;
using System.Diagnostics;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;
using System.Linq;

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
        public void can_iterate_through_instances_of_pipelineGraph_for_generics_if_not_registered()
        {
            PipelineGraph pipeline = new PipelineGraph(new PluginGraph());
            pipeline.InstancesOf(typeof(IService<>)).Count().ShouldEqual(0);
        }

        [Test]
        public void can_iterate_through_families_including_both_generics_and_normal()
        {
            Registry registry = new Registry();
            registry.ForRequestedType(typeof(IService<>))
                .AddConcreteType(typeof(Service<>))
                .AddConcreteType(typeof(Service2<>));

            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            PluginGraph graph = registry.Build();
            PipelineGraph pipeline = new PipelineGraph(graph);

            pipeline.PluginTypes.Count().ShouldEqual(2);
        }

        [Test]
        public void can_iterate_through_instance_of_pipelineGraph_for_generics()
        {
            Registry registry = new Registry();
            registry.ForRequestedType(typeof (IService<>))
                .AddConcreteType(typeof (Service<>))
                .AddConcreteType(typeof (Service2<>));

            var graph = registry.Build();
            var pipeline = new PipelineGraph(graph);

            foreach (var instance in pipeline.InstancesOf(typeof(IService<>)))
            {
                Debug.WriteLine(instance.Description);
            }

            pipeline.InstancesOf(typeof (IService<>)).Count().ShouldEqual(2);
        }

        [Test]
        public void can_iterate_through_instances_of_pipelineGraph_on_a_type_that_is_registered()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            PluginGraph graph = registry.Build();

            PipelineGraph pipeline = new PipelineGraph(graph);

            pipeline.InstancesOf(typeof(ISomething)).Count().ShouldEqual(2);

            pipeline.Inject<ISomething>(new SomethingTwo());

            pipeline.InstancesOf(typeof(ISomething)).Count().ShouldEqual(3);
        }

        [Test]
        public void can_iterate_through_empty_array_of_pipelineGraph_for_a_type_that_is_not_registered()
        {
            PluginGraph graph = new PluginGraph();
            PipelineGraph pipeline = new PipelineGraph(graph);

            pipeline.InstancesOf(typeof(ISomething)).Count().ShouldEqual(0);
        }


    }
}