using System.Linq;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Internals
{
    [TestFixture]
    public class StructureMapContainerFacilityTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For<IFileSystem>().Use<FileSystem>();
                x.For<IStreamingData>().Use(MockRepository.GenerateMock<IStreamingData>());
                x.For<IHttpWriter>().Use(new NulloHttpWriter());
                x.For<ICurrentChain>().Use(new CurrentChain(null, null));
                x.For<ICurrentHttpRequest>().Use(new StandInCurrentHttpRequest(){
                   
                    ApplicationRoot = "http://server"
                });

                x.For<IResourceHash>().Use(MockRepository.GenerateMock<IResourceHash>());
            });

            container.Configure(x => x.For<IContainerFacility>().Use<StructureMapContainerFacility>());


            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Route("/area/sub/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("/area/sub2/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("/area/sub3/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Models.ConvertUsing<ExampleConverter>().ConvertUsing<ExampleConverter2>();


                x.Services(s => s.AddService<IActivator>(new StubActivator()));
                x.Services(s => s.AddService<IActivator>(new StubActivator()));
                x.Services(s => s.AddService<IActivator>(new StubActivator()));
            });

            facility = new StructureMapContainerFacility(container);
            graph.As<IRegisterable>().Register(facility.Register);

            factory = facility.BuildFactory();
        }

        #endregion

        public class ExampleConverter : IConverterFamily
        {
            public bool Matches(PropertyInfo prop)
            {
                return true;
            }

            public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo prop)
            {
                return null;
            }
        }

        public class ExampleConverter2 : IConverterFamily
        {
            public bool Matches(PropertyInfo prop)
            {
                return true;
            }

            public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo prop)
            {
                return null;
            }
        }

        private Container container;
        private BehaviorGraph graph;
        private IServiceFactory factory;
        private StructureMapContainerFacility facility;

        [Test]
        public void can_return_all_the_registered_activators_smoke_test()
        {
            facility.GetAll<IActivator>().Count().ShouldEqual(3);
        }

        [Test]
        public void factory_should_be_itself()
        {
            factory.ShouldNotBeNull();
            factory.ShouldBeTheSameAs(facility);
        }

        [Test]
        public void register_a_service_by_value()
        {
            var myContainer = new Container();
            var myFacility = new StructureMapContainerFacility(myContainer);

            var registry = new TypeResolver();

            myFacility.Register(typeof (ITypeResolver), new ObjectDef{
                Value = registry
            });

            myFacility.BuildFactory();

            myContainer.GetInstance<ITypeResolver>().ShouldBeTheSameAs(registry);
        }

        [Test]
        public void should_be_able_to_create_the_basic_services_from_the_container()
        {
            container.GetInstance<IOutputWriter>().ShouldBeOfType<OutputWriter>();
        }

        [Test]
        public void should_be_able_to_inject_multiple_implementations_as_a_dependency()
        {
            var converterFamilies =
                container.GetInstance<BindingRegistry>().AllConverterFamilies();
            converterFamilies.ShouldContain(f => f.GetType() == typeof (ExampleConverter));
            converterFamilies.ShouldContain(f => f.GetType() == typeof (ExampleConverter2));
        }

        [Test]
        public void should_be_able_to_pull_all_of_the_route_behaviors_out_of_the_container()
        {
            container.GetAllInstances<IActionBehavior>().Count().ShouldEqual(3);
        }

        [Test]
        public void should_register_a_service_locator()
        {
            container.GetInstance<IServiceLocator>()
                .ShouldBeOfType<StructureMapServiceLocator>()
                .Container.ShouldBeTheSameAs(container);
        }

        [Test]
        public void standard_model_binder_should_not_be_registered_in_the_container()
        {
            container.GetAllInstances<IModelBinder>().Any(x => x is StandardModelBinder).ShouldBeFalse();
        }

    }
}