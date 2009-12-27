using System;
using System.Linq;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ModelQueryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new Registry();
            registry.ForRequestedType(typeof (IService<>))
                .AddConcreteType(typeof (Service<>))
                .AddConcreteType(typeof (Service2<>));

            registry.BuildInstancesOf<ISomething>()
                .TheDefaultIsConcreteType<SomethingOne>()
                .AddConcreteType<SomethingTwo>();

            registry.BuildInstancesOf<IServiceProvider>().AddConcreteType<MyDataSet>().AddConcreteType<MyDataView>();

            PluginGraph graph = registry.Build();
            var pipeline = new PipelineGraph(graph);

            _model = new Container(graph).Model;

            _container = new Container(graph);
        }

        #endregion

        public class MyDataSet : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }

        public class MyDataView : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }


        private IModel _model;
        private Container _container;


        [Test]
        public void HasImplementationFor()
        {
            _model.HasDefaultImplementationFor(typeof (ISomething)).ShouldBeTrue();
            _model.HasDefaultImplementationFor(GetType()).ShouldBeFalse();
            _model.HasDefaultImplementationFor(typeof (IServiceProvider)).ShouldBeFalse();
        }

        [Test]
        public void the_default_type_for()
        {
            _model.DefaultTypeFor<ISomething>().ShouldEqual(typeof (SomethingOne));
            _model.DefaultTypeFor<IWidget>().ShouldBeNull();

            _model.DefaultTypeFor(typeof (IService<>)).ShouldBeNull();
            _model.DefaultTypeFor(typeof(ISomething)).ShouldEqual(typeof(SomethingOne));
        }

        [Test]
        public void HasImplementationFor_w_container()
        {
            _container.Model.HasDefaultImplementationFor(typeof (ISomething)).ShouldBeTrue();
            _container.Model.HasDefaultImplementationFor(GetType()).ShouldBeFalse();
            _container.Model.HasDefaultImplementationFor(typeof (IServiceProvider)).ShouldBeFalse();
        }



        [Test]
        public void HasImplementationsFor()
        {
            _model.HasImplementationsFor(typeof (ISomething)).ShouldBeTrue();
            _model.HasImplementationsFor(GetType()).ShouldBeFalse();
            _model.HasImplementationsFor(typeof (IServiceProvider)).ShouldBeTrue();
        }

        [Test]
        public void HasImplementationsFor_w_container()
        {
            _container.Model.HasImplementationsFor(typeof (ISomething)).ShouldBeTrue();
            _container.Model.HasImplementationsFor(GetType()).ShouldBeFalse();
            _container.Model.HasImplementationsFor(typeof (IServiceProvider)).ShouldBeTrue();
        }

        [Test]
        public void iterate_over_instances_of_a_type()
        {
            _model.InstancesOf(typeof (IServiceProvider)).Count().ShouldEqual(2);
            _model.InstancesOf(typeof (IService<>)).Count().ShouldEqual(2);
            _model.InstancesOf<IServiceProvider>().Count().ShouldEqual(2);
        }

        [Test]
        public void iterate_over_instances_of_a_type_w_container()
        {
            _container.Model.InstancesOf(typeof (IServiceProvider)).Count().ShouldEqual(2);
            _container.Model.InstancesOf(typeof (IService<>)).Count().ShouldEqual(2);
            _container.Model.InstancesOf<IServiceProvider>().Count().ShouldEqual(2);
        }

        [Test]
        public void Iterate_over_pluginTypes()
        {
            // 3 registered plus the 4th is the IContainer itself
            _model.PluginTypes.Count().ShouldEqual(4);
        }

        [Test]
        public void Iterate_over_pluginTypes_w_container()
        {
            // IContainer is always added to the Container
            _container.Model.PluginTypes.Count().ShouldEqual(4);
        }
    }
}