using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Pipeline;
using System.Linq;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class InjectArrayTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public class Processor
        {
            private readonly IHandler[] _handlers;
            private readonly string _name;

            public Processor(IHandler[] handlers, string name)
            {
                _handlers = handlers;
                _name = name;
            }


            public IHandler[] Handlers { get { return _handlers; } }


            public string Name { get { return _name; } }
        }

        public class ProcessorWithList
        {
            private readonly IList<IHandler> _handlers;
            private readonly string _name;

            public ProcessorWithList(IList<IHandler> handlers, string name)
            {
                _handlers = handlers;
                _name = name;
            }


            public IList<IHandler> Handlers { get { return _handlers; } }


            public string Name { get { return _name; } }
        }

        public class Processor2
        {
            private readonly IHandler[] _first;
            private readonly IHandler[] _second;


            public Processor2(IHandler[] first, IHandler[] second)
            {
                _first = first;
                _second = second;
            }


            public IHandler[] First { get { return _first; } }

            public IHandler[] Second { get { return _second; } }
        }

        public interface IHandler
        {
        }

        public class Handler1 : IHandler
        {
        }

        public class Handler2 : IHandler
        {
        }

        public class Handler3 : IHandler
        {
        }

        [Test]
        public void CanStillAddOtherPropertiesAfterTheCallToChildArray()
        {
            var container = new Container(x =>
            {
                x.For<Processor>().Use<Processor>()
                    .TheArrayOf<IHandler>().Contains(
                    new SmartInstance<Handler1>(),
                    new SmartInstance<Handler2>(),
                    new SmartInstance<Handler3>()
                    )
                    .WithCtorArg("name").EqualTo("Jeremy");
            });

            container.GetInstance<Processor>().Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void get_a_configured_list()
        {
            var container = new Container(x =>
            {
                x.For<Processor>().Use<Processor>()
                    .TheArrayOf<IHandler>().Contains(
                    new SmartInstance<Handler1>(),
                    new SmartInstance<Handler2>(),
                    new SmartInstance<Handler3>()
                    )
                    .WithCtorArg("name").EqualTo("Jeremy");
            });

            container.GetInstance<Processor>().Handlers.Select(x => x.GetType()).ShouldHaveTheSameElementsAs(typeof(Handler1), typeof(Handler2), typeof(Handler3));
        }

        [Test]
        public void InjectPropertiesByName()
        {
            var container = new Container(r =>
            {
                r.For<Processor2>().Use<Processor2>()
                    .TheArrayOf<IHandler>("first").Contains(x =>
                    {
                        x.OfConcreteType<Handler1>();
                        x.OfConcreteType<Handler2>();
                    })
                    .TheArrayOf<IHandler>("second").Contains(x =>
                    {
                        x.OfConcreteType<Handler2>();
                        x.OfConcreteType<Handler3>();
                    });
            });


            var processor = container.GetInstance<Processor2>();

            processor.First[0].ShouldBeOfType<Handler1>();
            processor.First[1].ShouldBeOfType<Handler2>();
            processor.Second[0].ShouldBeOfType<Handler2>();
            processor.Second[1].ShouldBeOfType<Handler3>();
        }


        [Test]
        public void inline_definition_of_enumerable_child_respects_order_of_registration()
        {
            IContainer container = new Container(r =>
            {
                r.For<IHandler>().Add<Handler1>().Named("One");
                r.For<IHandler>().Add<Handler2>().Named("Two");

                r.For<Processor>().Use<Processor>()
                    .Ctor<string>("name").Is("Jeremy")
                    .EnumerableOf<IHandler>().Contains(x =>
                    {
                        x.TheInstanceNamed("Two");
                        x.TheInstanceNamed("One");
                    });
            });

            var processor = container.GetInstance<Processor>();
            processor.Handlers[0].ShouldBeOfType<Handler2>();
            processor.Handlers[1].ShouldBeOfType<Handler1>();
        }


        [Test]
        public void PlaceMemberInArrayByReference_with_SmartInstance()
        {
            IContainer manager = new Container(registry =>
            {
                registry.For<IHandler>().Add<Handler1>().Named("One");
                registry.For<IHandler>().Add<Handler2>().WithName("Two");


                registry.For<Processor>().Use<Processor>()
                    .WithCtorArg("name").EqualTo("Jeremy")
                    .TheArrayOf<IHandler>().Contains(x =>
                    {
                        x.TheInstanceNamed("Two");
                        x.TheInstanceNamed("One");
                    });
            });

            var processor = manager.GetInstance<Processor>();

            processor.Handlers[0].ShouldBeOfType<Handler2>();
            processor.Handlers[1].ShouldBeOfType<Handler1>();
        }

        [Test]
        public void ProgrammaticallyInjectArrayAllInline()
        {
            var container = new Container(x =>
            {
                x.For<Processor>().Use<Processor>()
                    .WithCtorArg("name").EqualTo("Jeremy")
                    .TheArrayOf<IHandler>().Contains(y =>
                    {
                        y.OfConcreteType<Handler1>();
                        y.OfConcreteType<Handler2>();
                        y.OfConcreteType<Handler3>();
                    });
            });


            var processor = container.GetInstance<Processor>();

            processor.Handlers[0].ShouldBeOfType<Handler1>();
            processor.Handlers[1].ShouldBeOfType<Handler2>();
            processor.Handlers[2].ShouldBeOfType<Handler3>();
        }

        [Test]
        public void ProgrammaticallyInjectArrayAllInline_with_smart_instance()
        {
            IContainer container = new Container(r =>
            {
                r.For<Processor>().Use<Processor>()
                    .WithCtorArg("name").EqualTo("Jeremy")
                    .TheArrayOf<IHandler>().Contains(x =>
                    {
                        x.OfConcreteType<Handler1>();
                        x.OfConcreteType<Handler2>();
                        x.OfConcreteType<Handler3>();
                    });
            });

            var processor = container.GetInstance<Processor>();

            processor.Handlers[0].ShouldBeOfType<Handler1>();
            processor.Handlers[1].ShouldBeOfType<Handler2>();
            processor.Handlers[2].ShouldBeOfType<Handler3>();
        }
    }
}