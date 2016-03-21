using Shouldly;
using StructureMap.Query;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;
using System;
using System.Linq;
using Xunit;

namespace StructureMap.Testing
{
    public class ModelQueryTester
    {
        public ModelQueryTester()
        {
            var registry = new Registry();
            registry.For(typeof(IService<>)).Add(typeof(Service<>));
            registry.For(typeof(IService<>)).Add(typeof(Service2<>));

            registry.For<ISomething>().Use<SomethingOne>();
            registry.For<ISomething>().Add<SomethingTwo>();

            registry.For<IServiceProvider>().Add<MyDataSet>();
            registry.For<IServiceProvider>().Add<MyDataView>();

            var graph = registry.Build();

            _model = new Container(graph).Model;

            _container = new Container(graph);
        }

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

        [Fact]
        public void HasImplementationFor()
        {
            _model.HasDefaultImplementationFor(typeof(ISomething)).ShouldBeTrue();
            _model.HasDefaultImplementationFor(GetType()).ShouldBeFalse();
            _model.HasDefaultImplementationFor(typeof(IServiceProvider)).ShouldBeFalse();
        }

        [Fact]
        public void HasImplementationFor_w_container()
        {
            _container.Model.HasDefaultImplementationFor(typeof(ISomething)).ShouldBeTrue();
            _container.Model.HasDefaultImplementationFor(GetType()).ShouldBeFalse();
            _container.Model.HasDefaultImplementationFor(typeof(IServiceProvider)).ShouldBeFalse();
        }

        [Fact]
        public void HasImplementationsFor()
        {
            _model.HasImplementationsFor(typeof(ISomething)).ShouldBeTrue();
            _model.HasImplementationsFor(GetType()).ShouldBeFalse();
            _model.HasImplementationsFor(typeof(IServiceProvider)).ShouldBeTrue();
        }

        [Fact]
        public void HasImplementationsFor_w_container()
        {
            _container.Model.HasImplementationsFor(typeof(ISomething)).ShouldBeTrue();
            _container.Model.HasImplementationsFor(GetType()).ShouldBeFalse();
            _container.Model.HasImplementationsFor(typeof(IServiceProvider)).ShouldBeTrue();
        }

        [Fact]
        public void iterate_over_instances_of_a_type()
        {
            _model.InstancesOf(typeof(IServiceProvider)).Count().ShouldBe(2);
            _model.InstancesOf(typeof(IService<>)).Count().ShouldBe(2);
            _model.InstancesOf<IServiceProvider>().Count().ShouldBe(2);
        }

        [Fact]
        public void iterate_over_instances_of_a_type_w_container()
        {
            _container.Model.InstancesOf(typeof(IServiceProvider)).Count().ShouldBe(2);
            _container.Model.InstancesOf(typeof(IService<>)).Count().ShouldBe(2);
            _container.Model.InstancesOf<IServiceProvider>().Count().ShouldBe(2);
        }

        [Fact]
        public void Iterate_over_pluginTypes()
        {
            // 3 registered plus the 4th is the IContainer itself + Func + Func w/ Arg + Lazy
            _model.PluginTypes.Count().ShouldBe(7);
        }

        [Fact]
        public void Iterate_over_pluginTypes_w_container()
        {
            // IContainer is always added to the Container + Func<> + Func<,> + Lazy<>
            _container.Model.PluginTypes.Count().ShouldBe(7);
        }

        [Fact]
        public void the_default_type_for()
        {
            _model.DefaultTypeFor<ISomething>().ShouldBe(typeof(SomethingOne));
            _model.DefaultTypeFor<IWidget>().ShouldBeNull();

            _model.DefaultTypeFor(typeof(IService<>)).ShouldBeNull();
            _model.DefaultTypeFor(typeof(ISomething)).ShouldBe(typeof(SomethingOne));
        }
    }
}