using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class InjectArrayTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ProgrammaticallyInjectArrayAllInline()
        {
            Registry registry = new Registry();

            registry.ForRequestedType<Processor>()
                .TheDefaultIs(
                Registry.Instance<Processor>().UsingConcreteType<Processor>()
                    .ChildArray<IHandler[]>().Contains(
                    Registry.Instance<IHandler>().UsingConcreteType<Handler1>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler2>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler3>()
                    )
                    .WithProperty("name").EqualTo("Jeremy")
                );

            IInstanceManager manager = registry.BuildInstanceManager();
            Processor processor = manager.CreateInstance<Processor>();

            Assert.IsInstanceOfType(typeof (Handler1), processor.Handlers[0]);
            Assert.IsInstanceOfType(typeof (Handler2), processor.Handlers[1]);
            Assert.IsInstanceOfType(typeof (Handler3), processor.Handlers[2]);
        }

        [Test]
        public void CanStillAddOtherPropertiesAfterTheCallToChildArray()
        {
            Registry registry = new Registry();

            registry.ForRequestedType<Processor>()
                .TheDefaultIs(
                Registry.Instance<Processor>().UsingConcreteType<Processor>()
                    .ChildArray<IHandler[]>().Contains(
                    Registry.Instance<IHandler>().UsingConcreteType<Handler1>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler2>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler3>()
                    )
                    .WithProperty("name").EqualTo("Jeremy")
                );

            IInstanceManager manager = registry.BuildInstanceManager();
            Processor processor = manager.CreateInstance<Processor>();

            Assert.AreEqual("Jeremy", processor.Name);
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             ExpectedMessage =
                 "StructureMap Exception Code:  307\nIn the call to ChildArray<T>(), the type T must be an array")]
        public void TryToInjectByTheElementTypeInsteadOfTheArrayType()
        {
            Registry registry = new Registry();

            registry.ForRequestedType<Processor>()
                .TheDefaultIs(
                Registry.Instance<Processor>().UsingConcreteType<Processor>()
                    .WithProperty("name").EqualTo("Jeremy")
                    .ChildArray<IHandler>().Contains(
                    Registry.Instance<IHandler>().UsingConcreteType<Handler1>())
                );
        }


        [Test,
         ExpectedException(typeof (StructureMapException),
             ExpectedMessage =
                 "StructureMap Exception Code:  307\nIn the call to ChildArray<T>(), the type T must be an array")]
        public void InjectPropertiesByNameButUseTheElementType()
        {
            Registry registry = new Registry();

            registry.ForRequestedType<Processor2>()
                .TheDefaultIs(
                Registry.Instance<Processor2>().UsingConcreteType<Processor2>()
                    .ChildArray<IHandler>("first").Contains(
                    Registry.Instance<IHandler>().UsingConcreteType<Handler1>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler2>()
                    )
                    .ChildArray<IHandler[]>("second").Contains(
                    Registry.Instance<IHandler>().UsingConcreteType<Handler2>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler3>()
                    )
                );
        }

        [Test]
        public void InjectPropertiesByName()
        {
            Registry registry = new Registry();

            registry.ForRequestedType<Processor2>()
                .TheDefaultIs(
                Registry.Instance<Processor2>().UsingConcreteType<Processor2>()
                    .ChildArray<IHandler[]>("first").Contains(
                    Registry.Instance<IHandler>().UsingConcreteType<Handler1>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler2>()
                    )
                    .ChildArray<IHandler[]>("second").Contains(
                    Registry.Instance<IHandler>().UsingConcreteType<Handler2>(),
                    Registry.Instance<IHandler>().UsingConcreteType<Handler3>()
                    )
                );

            IInstanceManager manager = registry.BuildInstanceManager();
            Processor2 processor = manager.CreateInstance<Processor2>();

            Assert.IsInstanceOfType(typeof (Handler1), processor.First[0]);
            Assert.IsInstanceOfType(typeof (Handler2), processor.First[1]);
            Assert.IsInstanceOfType(typeof (Handler2), processor.Second[0]);
            Assert.IsInstanceOfType(typeof (Handler3), processor.Second[1]);
        }

        [Test]
        public void PlaceMemberInArrayByReference()
        {
            Registry registry = new Registry();
            registry.AddInstanceOf<IHandler>().UsingConcreteType<Handler1>().WithName("One");
            registry.AddInstanceOf<IHandler>().UsingConcreteType<Handler2>().WithName("Two");

            registry.ForRequestedType<Processor>()
                .TheDefaultIs(
                    Registry.Instance<Processor>().UsingConcreteType<Processor>()
                        .WithProperty("name").EqualTo("Jeremy")
                        .ChildArray<IHandler[]>().Contains(
                            Registry.Instance("Two"),
                            Registry.Instance("One") 
                        )
                );

            IInstanceManager manager = registry.BuildInstanceManager();
            Processor processor = manager.CreateInstance<Processor>();

            Assert.IsInstanceOfType(typeof(Handler2), processor.Handlers[0]);
            Assert.IsInstanceOfType(typeof(Handler1), processor.Handlers[1]);
        }


        public class Processor
        {
            private readonly IHandler[] _handlers;
            private readonly string _name;

            public Processor(IHandler[] handlers, string name)
            {
                _handlers = handlers;
                _name = name;
            }


            public IHandler[] Handlers
            {
                get { return _handlers; }
            }


            public string Name
            {
                get { return _name; }
            }
        }

        public class Processor2
        {
            private IHandler[] _first;
            private IHandler[] _second;


            public Processor2(IHandler[] first, IHandler[] second)
            {
                _first = first;
                _second = second;
            }


            public IHandler[] First
            {
                get { return _first; }
            }

            public IHandler[] Second
            {
                get { return _second; }
            }
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
    }
}