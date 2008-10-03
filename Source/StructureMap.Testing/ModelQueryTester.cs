using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Graph;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ModelQueryTester
    {
        private Model _model;
        private Container _container;

        [SetUp]
        public void SetUp()
        {
            Registry registry = new Registry();
            registry.ForRequestedType(typeof(IService<>))
                .AddConcreteType(typeof(Service<>))
                .AddConcreteType(typeof(Service2<>));

            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            registry.BuildInstancesOf<IServiceProvider>().AddConcreteType<DataSet>().AddConcreteType<DataView>();

            PluginGraph graph = registry.Build();
            PipelineGraph pipeline = new PipelineGraph(graph);

            _model = new Model(pipeline);

            _container = new Container(graph);

        }


        [Test]
        public void HasImplementationFor()
        {
            _model.HasDefaultImplementationFor(typeof(ISomething)).ShouldBeTrue();
            _model.HasDefaultImplementationFor(GetType()).ShouldBeFalse();
            _model.HasDefaultImplementationFor(typeof(IServiceProvider)).ShouldBeFalse();
        }

        [Test]
        public void HasImplementationsFor()
        {
            _model.HasImplementationsFor(typeof(ISomething)).ShouldBeTrue();
            _model.HasImplementationsFor(GetType()).ShouldBeFalse();
            _model.HasImplementationsFor(typeof(IServiceProvider)).ShouldBeTrue();
        }

        [Test]
        public void Iterate_over_pluginTypes()
        {
            _model.PluginTypes.Count().ShouldEqual(3);
        }

        [Test]
        public void iterate_over_instances_of_a_type()
        {
            _model.InstancesOf(typeof (IServiceProvider)).Count().ShouldEqual(2);
            _model.InstancesOf(typeof (IService<>)).Count().ShouldEqual(2);
            _model.InstancesOf<IServiceProvider>().Count().ShouldEqual(2);
        }

        [Test]
        public void HasImplementationFor_w_container()
        {
            _container.Model.HasDefaultImplementationFor(typeof(ISomething)).ShouldBeTrue();
            _container.Model.HasDefaultImplementationFor(GetType()).ShouldBeFalse();
            _container.Model.HasDefaultImplementationFor(typeof(IServiceProvider)).ShouldBeFalse();
        }

        [Test]
        public void HasImplementationsFor_w_container()
        {
            _container.Model.HasImplementationsFor(typeof(ISomething)).ShouldBeTrue();
            _container.Model.HasImplementationsFor(GetType()).ShouldBeFalse();
            _container.Model.HasImplementationsFor(typeof(IServiceProvider)).ShouldBeTrue();
        }

        [Test]
        public void Iterate_over_pluginTypes_w_container()
        {
            _container.Model.PluginTypes.Count().ShouldEqual(3);
        }

        [Test]
        public void iterate_over_instances_of_a_type_w_container()
        {
            _container.Model.InstancesOf(typeof (IServiceProvider)).Count().ShouldEqual(2);
            _container.Model.InstancesOf(typeof (IService<>)).Count().ShouldEqual(2);
            _container.Model.InstancesOf<IServiceProvider>().Count().ShouldEqual(2);
        }
    }
}
