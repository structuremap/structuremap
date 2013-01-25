using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
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

        private IInstanceFactory getISomethingFactory()
        {
            var family = new PluginFamily(typeof (ISomething));
            return new InstanceFactory(family);
        }

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

        [PluginFamily("Default")]
        public interface IThingy
        {
        }

        [Pluggable("Default")]
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
        public void Add_an_assembly_in_the_Configure()
        {
            var container = new Container();

            container.Configure(registry => { registry.Scan(x => x.TheCallingAssembly()); });


            container.GetInstance<IThingy>().ShouldBeOfType<TheThingy>();
        }

        [Test]
        public void Add_an_assembly_on_the_fly_and_pick_up_plugins()
        {
            var container = new Container();
            container.Configure(
                registry => registry.Scan(x =>
                {
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
                registry =>
                {
                    registry.Scan(x =>
                    {
                        x.TheCallingAssembly();
                        x.AddAllTypesOf(typeof (IWidget));
                    });
                }
                );

            IList<IWidget> instances = container.GetAllInstances<IWidget>();
            bool found = false;
            foreach (IWidget widget in instances)
            {
                found |= widget.GetType().Equals(typeof (TheWidget));
            }

            Assert.IsTrue(found);
        }

        [Test]
        public void Add_an_assembly_on_the_fly_and_pick_up_plugins4()
        {
            var container = new Container();
            container.Configure(
                registry => registry.Scan(
                                x =>
                                    {
                                        x.AssemblyContainingType(typeof (IOtherService<>));
                                        x.AddAllTypesOf(typeof (IOtherService<>));
                                    }));

            var instances = container.GetAllInstances<IOtherService<string>>();
            instances.Any(s=> s is Service4).ShouldBeTrue();
        }

        [Test]
        public void Add_generic_stuff_in_configure()
        {
            var container = new Container();
            container.Configure(registry =>
            {
                registry.ForRequestedType(typeof (IService<>))
                    .AddConcreteType(typeof (Service1<>))
                    .AddConcreteType(typeof (Service2<>));
            });

            Assert.AreEqual(2, container.GetAllInstances<IService<string>>().Count);
        }

        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists()
        {
            var container = new Container(x => { x.For<ISomething>().TheDefaultIsConcreteType<SomethingTwo>(); });

            container.Configure(x => { x.For<ISomething>().TheDefaultIsConcreteType<SomethingOne>(); });

            container.GetInstance<ISomething>().ShouldBeOfType<SomethingOne>();
        }


        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists2()
        {
            var container = new Container(x => { x.For<ISomething>(); });

            container.Configure(x => { x.For<ISomething>().TheDefaultIsConcreteType<SomethingOne>(); });

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
            container.Configure(r =>
            {
                r.ForRequestedType<IService>().AddInstances(x =>
                {
                    x.Object(_red).WithName("Red");
                    x.Object(_blue).WithName("Blue");
                });
            });

            Assert.AreSame(_red, container.GetInstance(typeof (IService), "Red"));
            Assert.AreSame(_blue, container.GetInstance(typeof (IService), "Blue"));
        }


        [Test]
        public void AddNamedInstanceByType()
        {
            var container = new Container(r =>
            {
                r.ForRequestedType<ISomething>().AddInstances(x =>
                {
                    x.OfConcreteType<SomethingOne>().WithName("One");
                    x.OfConcreteType<SomethingTwo>().WithName("Two");
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

            var container = new Container(r =>
            {
                r.ForRequestedType<ISomething>().AddInstances(x =>
                {
                    x.Object(one).WithName("One");
                    x.Object(two).WithName("Two");
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

            IList<ISomething> list = container.GetAllInstances<ISomething>();

            Assert.AreEqual(1, list.Count);
            list[0].ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void AddTypeThroughContainer()
        {
            var container = new Container(x => { x.For<ISomething>().TheDefaultIsConcreteType<SomethingOne>(); });

            container.GetInstance<ISomething>().ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void CanAddInstancesDirectlyToAnInstanceFactory()
        {
            IInstanceFactory factory = getISomethingFactory();

            factory.AddInstance(new ObjectInstance(_red).WithName("Red"));
            factory.AddInstance(new ObjectInstance(_blue).WithName("Blue"));

            factory.FindInstance("Red").ShouldNotBeNull();
        }

        [Test]
        public void InjectType()
        {
            var container = new Container(
                r => r.For<ISomething>().Use<SomethingOne>());

            IList<ISomething> list = container.GetAllInstances<ISomething>();

            list[0].ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void JustAddATypeWithNoNameAndDefault()
        {
            var container = new Container(x => { x.For<ISomething>().TheDefaultIsConcreteType<SomethingOne>(); });

            container.GetInstance<ISomething>().ShouldBeOfType<SomethingOne>();
        }

        [Test]
        public void NowOverwriteAPreviouslyAttachedInstance()
        {
            IInstanceFactory factory = getISomethingFactory();

            factory.AddInstance(new ObjectInstance(_red).WithName("Red"));
            ObjectInstance oldBlue = new ObjectInstance(_blue).WithName("Blue");
            factory.AddInstance(oldBlue);

            // Replace Blue
            ObjectInstance newBlue = new ObjectInstance(_orange).WithName("Blue");
            factory.AddInstance(newBlue);

            factory.FindInstance("Blue").ShouldBeTheSameAs(newBlue);

            factory.AllInstances.Contains(oldBlue).ShouldBeFalse();
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
