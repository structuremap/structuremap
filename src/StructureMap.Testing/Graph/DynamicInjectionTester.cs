using System;
using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class DynamicInjectionTester
    {
        private readonly IService _red = new ColorService("Red");
        private readonly IService _blue = new ColorService("Blue");
        private readonly IService _orange = new ColorService("Orange");


        public interface IService<T>
        {
        }

        public class Service1<T> : IService<T>
        {
        }

        public class Service2<T> : IService<T>
        {
        }

        public class Service3<T> : IService<T>
        {
        }

        public interface IOtherService<T>
        {
        }

        public class Service4 : IOtherService<string>
        {
        }

        //[PluginFamily("Default")]
        public interface IThingy
        {
        }

        //[Pluggable("Default")]
        public class TheThingy : IThingy
        {
        }


        public class TheWidget : IWidget
        {
            #region IWidget Members

            public void DoSomething()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists()
        {
            var container = new Container(x => x.For<ISomething>().Use<SomethingTwo>());

            container.Configure(x => x.For<ISomething>().Use<SomethingOne>());

            container.GetInstance<ISomething>().ShouldBeOfType<SomethingOne>();
        }


        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists2()
        {
            var container = new Container(x => { x.For<ISomething>(); });

            container.Configure(x => { x.For<ISomething>().Use<SomethingOne>(); });

            container.GetInstance<ISomething>().ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void AddInstanceFromContainer()
        {
            var one = new SomethingOne();
            var container = new Container();
            container.Inject<ISomething>(one);

            Assert.AreSame(one, container.GetInstance<ISomething>());
        }

        [Test]
        public void AddInstanceToInstanceManagerWhenTheInstanceFactoryDoesNotExist()
        {
            IContainer container = new Container(new PluginGraph());
            container.Configure(r => {
                r.For<IService>().AddInstances(x => {
                    x.Object(_red).Named("Red");
                    x.Object(_blue).Named("Blue");
                });
            });

            Assert.AreSame(_red, container.GetInstance(typeof (IService), "Red"));
            Assert.AreSame(_blue, container.GetInstance(typeof (IService), "Blue"));
        }


        [Test]
        public void AddNamedInstanceByType()
        {
            var container = new Container(r => {
                r.For<ISomething>().AddInstances(x => {
                    x.Type<SomethingOne>().Named("One");
                    x.Type<SomethingTwo>().Named("Two");
                });
            });

            container.GetInstance<ISomething>("One").ShouldBeOfType<SomethingOne>();
            container.GetInstance<ISomething>("Two").ShouldBeOfType<SomethingTwo>();
        }

        [Test]
        public void AddNamedInstanceToobjectFactory()
        {
            var one = new SomethingOne();
            var two = new SomethingOne();

            var container = new Container(r => {
                r.For<ISomething>().AddInstances(x => {
                    x.Object(one).Named("One");
                    x.Object(two).Named("Two");
                });
            });

            Assert.AreSame(one, container.GetInstance<ISomething>("One"));
            Assert.AreSame(two, container.GetInstance<ISomething>("Two"));
        }


        [Test]
        public void AddPluginForTypeWhenThePluginDoesNotAlreadyExistsDoesNothing()
        {
            var pluginGraph = new PluginGraph();
            IContainer container = new Container(pluginGraph);
            container.Configure(
                r => { r.For<ISomething>().Use<SomethingOne>(); });

            container.GetAllInstances<ISomething>()
                .Single()
                .ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void AddTypeThroughContainer()
        {
            var container = new Container(x => { x.For<ISomething>().Use<SomethingOne>(); });

            container.GetInstance<ISomething>().ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void Add_an_assembly_in_the_Configure()
        {
            var container = new Container();

            container.Configure(registry => {
                registry.Scan(x => {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<IThingy>();
                });
            });


            container.GetInstance<IThingy>().ShouldBeOfType<TheThingy>();
        }

        [Test]
        public void Add_an_assembly_on_the_fly_and_pick_up_plugins()
        {
            var container = new Container();
            container.Configure(
                registry => registry.Scan(x => {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<IWidget>();
                }));

            container.GetAllInstances<IWidget>().OfType<TheWidget>().Any().ShouldBeTrue();
        }

        [Test]
        public void Add_an_assembly_on_the_fly_and_pick_up_plugins3()
        {
            var container = new Container();

            container.Configure(
                registry => {
                    registry.Scan(x => {
                        x.TheCallingAssembly();
                        x.AddAllTypesOf(typeof (IWidget));
                    });
                }
                );

            container.GetAllInstances<IWidget>()
                .OfType<TheWidget>()
                .Any().ShouldBeTrue();
        }

        [Test]
        public void Add_an_assembly_on_the_fly_and_pick_up_plugins4()
        {
            var container = new Container();
            container.Configure(
                registry => registry.Scan(
                    x => {
                        x.AssemblyContainingType(typeof (IOtherService<>));
                        x.AddAllTypesOf(typeof (IOtherService<>));
                    }));

            var instances = container.GetAllInstances<IOtherService<string>>();
            instances.Any(s => s is Service4).ShouldBeTrue();
        }

        [Test]
        public void Add_generic_stuff_in_configure()
        {
            var container = new Container();
            container.Configure(registry => {
                registry.For(typeof (IService<>)).Add(typeof (Service1<>));
                registry.For(typeof (IService<>)).Add(typeof (Service2<>));
            });

            container.GetAllInstances<IService<string>>().Count().ShouldEqual(2);
        }


        [Test]
        public void InjectType()
        {
            var container = new Container(
                r => r.For<ISomething>().Use<SomethingOne>());

            container.GetAllInstances<ISomething>()
                .Single()
                .ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void JustAddATypeWithNoNameAndDefault()
        {
            var container = new Container(x => { x.For<ISomething>().Use<SomethingOne>(); });

            container.GetInstance<ISomething>().ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void OverwriteInstanceFromObjectFactory()
        {
            var one = new SomethingOne();
            var two = new SomethingOne();
            var container = new Container();
            container.Inject<ISomething>(one);
            container.Inject<ISomething>(two);

            Assert.AreSame(two, container.GetInstance<ISomething>());
        }
    }


    public class SomethingOne : ISomething
    {
        public void Go()
        {
            throw new NotImplementedException();
        }
    }

    public class SomethingTwo : ISomething
    {
        public void Go()
        {
            throw new NotImplementedException();
        }
    }
}