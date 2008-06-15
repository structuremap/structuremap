using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Testing.Pipeline;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ExplicitArgumentTester : RegistryExpressions
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
            StructureMapConfiguration.UseDefaultStructureMapConfigFile = false;
        }

        [TearDown]
        public void TearDown()
        {
            StructureMapConfiguration.ResetAll();
            ObjectFactory.Reset();
        }

        #endregion

        public interface IExplicitTarget
        {
        }

        public class RedTarget : IExplicitTarget
        {
        }

        public class GreenTarget : IExplicitTarget
        {
        }

        public class ExplicitTarget : IExplicitTarget
        {
            private readonly string _name;
            private readonly IProvider _provider;

            public ExplicitTarget(string name, IProvider provider)
            {
                _name = name;
                _provider = provider;
            }


            public string Name
            {
                get { return _name; }
            }

            public IProvider Provider
            {
                get { return _provider; }
            }
        }

        public interface IProvider
        {
        }

        public class RedProvider : IProvider
        {
        }

        public class BlueProvider : IProvider
        {
        }

        [Test]
        public void ExplicitArguments_can_return_child_by_name()
        {
            ExplicitArguments args = new ExplicitArguments();
            Node theNode = new Node();
            args.SetArg("node", theNode);

            IConfiguredInstance instance = new ExplicitInstance<Command>(args, null);

            Assert.AreSame(theNode, instance.GetChild("node", typeof (Node), new StubBuildSession()));
        }

        [Test]
        public void Fill_in_argument_by_name()
        {
            Container manager = new Container();
            manager.SetDefault<IView, View>();

            Node theNode = new Node();
            Trade theTrade = new Trade();

            Command command = manager
                .With("node").EqualTo(theNode)
                .With(theTrade)
                .GetInstance<Command>();

            Assert.IsInstanceOfType(typeof (View), command.View);
            Assert.AreSame(theNode, command.Node);
            Assert.AreSame(theTrade, command.Trade);
        }

        [Test]
        public void NowDoItWithObjectFactoryItself()
        {
            StructureMapConfiguration.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                Instance<ExplicitTarget>()
                    .UsingConcreteType<ExplicitTarget>()
                    .Child<IProvider>().IsConcreteType<RedProvider>()
                    .WithProperty("name").EqualTo("Jeremy")
                );

            ObjectFactory.Reset();

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ExplicitTarget firstTarget = ObjectFactory.GetInstance<ExplicitTarget>();
            Assert.IsInstanceOfType(typeof (RedProvider), firstTarget.Provider);

            // Now, set the explicit arg for IProvider
            BlueProvider theBlueProvider = new BlueProvider();
            ExplicitTarget secondTarget = ObjectFactory.With<IProvider>(theBlueProvider).GetInstance<ExplicitTarget>();
            Assert.AreSame(theBlueProvider, secondTarget.Provider);
        }

        [Test]
        public void OverrideAPrimitiveWithObjectFactory()
        {
            StructureMapConfiguration.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                Instance<ExplicitTarget>()
                    .UsingConcreteType<ExplicitTarget>()
                    .Child<IProvider>().IsConcreteType<RedProvider>()
                    .WithProperty("name").EqualTo("Jeremy")
                );

            ObjectFactory.Reset();

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ExplicitTarget firstTarget = ObjectFactory.GetInstance<ExplicitTarget>();
            Assert.AreEqual("Jeremy", firstTarget.Name);

            // Now, set the explicit arg for IProvider
            ExplicitTarget secondTarget = ObjectFactory.With("name").EqualTo("Julia").GetInstance<ExplicitTarget>();
            Assert.AreEqual("Julia", secondTarget.Name);
        }

        [Test]
        public void Pass_in_arguments_as_dictionary()
        {
            Container manager = new Container();
            manager.SetDefault<IView, View>();

            Node theNode = new Node();
            Trade theTrade = new Trade();

            ExplicitArguments args = new ExplicitArguments();
            args.Set(theNode);
            args.SetArg("trade", theTrade);

            Command command = manager.GetInstance<Command>(args);

            Assert.IsInstanceOfType(typeof (View), command.View);
            Assert.AreSame(theNode, command.Node);
            Assert.AreSame(theTrade, command.Trade);
        }


        [Test]
        public void PassAnArgumentIntoExplicitArgumentsForARequestedInterface()
        {
            IContainer manager =
                new Container(
                    registry => registry.ForRequestedType<IProvider>().TheDefaultIsConcreteType<LumpProvider>());

            ExplicitArguments args = new ExplicitArguments();
            Lump theLump = new Lump();
            args.Set(theLump);

            LumpProvider instance = (LumpProvider) manager.GetInstance<IProvider>(args);
            Assert.AreSame(theLump, instance.Lump);
        }

        [Test]
        public void PassAnArgumentIntoExplicitArgumentsForARequestedInterfaceUsingObjectFactory()
        {
            StructureMapConfiguration.ForRequestedType<IProvider>().TheDefaultIsConcreteType<LumpProvider>();
            ObjectFactory.Reset();
            Lump theLump = new Lump();

            LumpProvider provider = (LumpProvider) ObjectFactory.With(theLump).GetInstance<IProvider>();
            Assert.AreSame(theLump, provider.Lump);
        }

        [Test]
        public void PassAnArgumentIntoExplicitArgumentsThatMightNotAlreadyBeRegistered()
        {
            Lump theLump = new Lump();
            LumpProvider provider = ObjectFactory.With(theLump).GetInstance<LumpProvider>();
            Assert.AreSame(theLump, provider.Lump);
        }

        [Test]
        public void PassExplicitArgsIntoInstanceManager()
        {
            IContainer manager = new Container(registry => registry.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                                                               Instance<ExplicitTarget>()
                                                                   .UsingConcreteType<ExplicitTarget>()
                                                                   .Child<IProvider>().IsConcreteType<RedProvider>()
                                                                   .WithProperty("name").EqualTo("Jeremy")
                                                               ));

            ExplicitArguments args = new ExplicitArguments();

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ExplicitTarget firstTarget = manager.GetInstance<ExplicitTarget>(args);
            Assert.IsInstanceOfType(typeof (RedProvider), firstTarget.Provider);

            // Now, set the explicit arg for IProvider
            args.Set<IProvider>(new BlueProvider());
            ExplicitTarget secondTarget = manager.GetInstance<ExplicitTarget>(args);
            Assert.IsInstanceOfType(typeof (BlueProvider), secondTarget.Provider);
        }

        [Test]
        public void RegisterAndFindServicesOnTheExplicitArgument()
        {
            ExplicitArguments args = new ExplicitArguments();
            Assert.IsNull(args.Get<IProvider>());

            RedProvider red = new RedProvider();
            args.Set<IProvider>(red);

            Assert.AreSame(red, args.Get<IProvider>());

            args.Set<IExplicitTarget>(new RedTarget());
            Assert.IsInstanceOfType(typeof (RedTarget), args.Get<IExplicitTarget>());
        }

        [Test]
        public void RegisterAndRetrieveArgs()
        {
            ExplicitArguments args = new ExplicitArguments();
            Assert.IsNull(args.GetArg("name"));

            args.SetArg("name", "Jeremy");
            Assert.AreEqual("Jeremy", args.GetArg("name"));

            args.SetArg("age", 34);
            Assert.AreEqual(34, args.GetArg("age"));
        }
    }

    public class Lump
    {
    }

    public class LumpProvider : ExplicitArgumentTester.IProvider
    {
        private readonly Lump _lump;

        public LumpProvider(Lump lump)
        {
            _lump = lump;
        }


        public Lump Lump
        {
            get { return _lump; }
        }
    }


    public class Trade
    {
    }

    public class Node
    {
    }

    public interface IView
    {
    }

    public class View : IView
    {
    }

    public class Command
    {
        private readonly Node _node;
        private readonly Trade _trade;
        private readonly IView _view;

        public Command(Trade trade, Node node, IView view)
        {
            _trade = trade;
            _node = node;
            _view = view;
        }

        public Trade Trade
        {
            get { return _trade; }
        }

        public Node Node
        {
            get { return _node; }
        }

        public IView View
        {
            get { return _view; }
        }
    }
}