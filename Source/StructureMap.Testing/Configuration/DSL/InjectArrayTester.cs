using NUnit.Framework;
using StructureMap.Pipeline;

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
                x.ForRequestedType<Processor>().TheDefault.Is
                    .OfConcreteType<Processor>()
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
        public void InjectPropertiesByName()
        {
            var container = new Container(r =>
            {
                r.ForRequestedType<Processor2>().TheDefault.Is.OfConcreteType<Processor2>()
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

            Assert.IsInstanceOfType(typeof (Handler1), processor.First[0]);
            Assert.IsInstanceOfType(typeof (Handler2), processor.First[1]);
            Assert.IsInstanceOfType(typeof (Handler2), processor.Second[0]);
            Assert.IsInstanceOfType(typeof (Handler3), processor.Second[1]);
        }


        [Test]
        public void PlaceMemberInArrayByReference()
        {
            IContainer manager = new Container(r =>
            {
                r.InstanceOf<IHandler>().Is.OfConcreteType<Handler1>().WithName("One");
                r.InstanceOf<IHandler>().Is.OfConcreteType<Handler2>().WithName("Two");

                r.ForRequestedType<Processor>().TheDefault.Is.OfConcreteType<Processor>()
                    .WithCtorArg("name").EqualTo("Jeremy")
                    .TheArrayOf<IHandler>().Contains(x =>
                    {
                        x.TheInstanceNamed("Two");
                        x.TheInstanceNamed("One");
                    });
            });

            var processor = manager.GetInstance<Processor>();

            Assert.IsInstanceOfType(typeof (Handler2), processor.Handlers[0]);
            Assert.IsInstanceOfType(typeof (Handler1), processor.Handlers[1]);
        }


        [Test]
        public void PlaceMemberInArrayByReference_with_SmartInstance()
        {
            IContainer manager = new Container(registry =>
            {
                registry.InstanceOf<IHandler>().Is.OfConcreteType<Handler1>().WithName("One");
                registry.InstanceOf<IHandler>().Is.OfConcreteType<Handler2>().WithName("Two");

                registry.ForRequestedType<Processor>().TheDefault.Is.OfConcreteType<Processor>()
                    .WithCtorArg("name").EqualTo("Jeremy")
                    .TheArrayOf<IHandler>().Contains(x =>
                    {
                        x.TheInstanceNamed("Two");
                        x.TheInstanceNamed("One");
                    });
            });

            var processor = manager.GetInstance<Processor>();

            Assert.IsInstanceOfType(typeof (Handler2), processor.Handlers[0]);
            Assert.IsInstanceOfType(typeof (Handler1), processor.Handlers[1]);
        }

        [Test]
        public void ProgrammaticallyInjectArrayAllInline()
        {
            var container = new Container(x =>
            {
                x.ForRequestedType<Processor>().TheDefault.Is.OfConcreteType<Processor>()
                    .WithCtorArg("name").EqualTo("Jeremy")
                    .TheArrayOf<IHandler>().Contains(y =>
                    {
                        y.OfConcreteType<Handler1>();
                        y.OfConcreteType<Handler2>();
                        y.OfConcreteType<Handler3>();
                    });
            });


            var processor = container.GetInstance<Processor>();

            Assert.IsInstanceOfType(typeof (Handler1), processor.Handlers[0]);
            Assert.IsInstanceOfType(typeof (Handler2), processor.Handlers[1]);
            Assert.IsInstanceOfType(typeof (Handler3), processor.Handlers[2]);
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

            Assert.IsInstanceOfType(typeof (Handler1), processor.Handlers[0]);
            Assert.IsInstanceOfType(typeof (Handler2), processor.Handlers[1]);
            Assert.IsInstanceOfType(typeof (Handler3), processor.Handlers[2]);
        }
    }
}