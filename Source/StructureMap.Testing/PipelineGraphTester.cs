using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;

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

        private void expectVisits(Registry registry, Action<IPipelineGraphVisitor> action)
        {
            MockRepository mocks = new MockRepository();
            IPipelineGraphVisitor visitor = mocks.CreateMock<IPipelineGraphVisitor>();

            using (mocks.Record())
            {
                action(visitor);
            }

            using (mocks.Playback())
            {
                PluginGraph graph = registry.Build();
                PipelineGraph pipeline = new PipelineGraph(graph);

                pipeline.Visit(visitor);
            }
        }


        [Test]
        public void Visit_a_single_family_with_a_default_and_another_instance()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            expectVisits(registry, visitor =>
            {
                visitor.PluginType(typeof (ISomething), null, null);
                LastCall.Constraints(Is.Equal(typeof (ISomething)), Is.TypeOf(typeof (ConfiguredInstance)), Is.TypeOf<BuildPolicy>());

                visitor.Instance(typeof (ISomething), null);
                LastCall.Constraints(Is.Equal(typeof(ISomething)), Is.TypeOf(typeof(ConfiguredInstance)))
                    .Repeat.Twice();
            });
        }

        [Test]
        public void Visit_a_single_family_with_no_default()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<ISomething>()
                .AddConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            expectVisits(registry, visitor =>
            {
                visitor.PluginType(typeof(ISomething), null, new BuildPolicy());
                visitor.Instance(typeof (ISomething), null);
                LastCall.Constraints(Is.Equal(typeof (ISomething)), Is.TypeOf(typeof (ConfiguredInstance)))
                    .Repeat.Twice();
            });
        }

        [Test]
        public void Visit_a_single_family_with_only_a_default()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>();

            expectVisits(registry, visitor =>
            {
                visitor.PluginType(typeof(ISomething), null, null);
                LastCall.Constraints(Is.Equal(typeof (ISomething)), Is.TypeOf(typeof (ConfiguredInstance)), Is.TypeOf<BuildPolicy>());

                visitor.Instance(typeof (ISomething), null);
                LastCall.Constraints(Is.Equal(typeof (ISomething)), Is.TypeOf(typeof (ConfiguredInstance)));
            });
        }


        [Test]
        public void Visit_three_families()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<ISomething>();
            registry.BuildInstancesOf<IWidget>();
            registry.BuildInstancesOf<Rule>();

            expectVisits(registry, visitor =>
            {
                visitor.PluginType(typeof(ISomething), null, new BuildPolicy());
                visitor.PluginType(typeof(IWidget), null, new BuildPolicy());
                visitor.PluginType(typeof(Rule), null, new BuildPolicy());
            });
        }
    }
}