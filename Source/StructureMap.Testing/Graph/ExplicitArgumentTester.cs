using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Pipeline;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ExplicitArgumentTester
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
        public void Example()
        {
            IContainer container = new Container();
            var theTrade = new Trade();

            var view = container.With(theTrade).GetInstance<TradeView>();

            view.Trade.ShouldBeTheSameAs(theTrade);
        }

        [Test]
        public void Explicit_services_are_used_throughout_the_object_graph()
        {
            var theTrade = new Trade();

            IContainer container = new Container(r =>
            {
                r.ForRequestedType<IView>().TheDefaultIsConcreteType<TradeView>();
                r.ForRequestedType<Node>().TheDefaultIsConcreteType<TradeNode>();
            });

            var command = container.With(theTrade).GetInstance<Command>();

            command.Trade.ShouldBeTheSameAs(theTrade);
            command.Node.IsType<TradeNode>().Trade.ShouldBeTheSameAs(theTrade);
            command.View.IsType<TradeView>().Trade.ShouldBeTheSameAs(theTrade);
        }

        [Test]
        public void ExplicitArguments_can_return_child_by_name()
        {
            var args = new ExplicitArguments();
            var theNode = new Node();
            args.SetArg("node", theNode);

            IConfiguredInstance instance = new ExplicitInstance(typeof (Command), args, null);

            Assert.AreSame(theNode, instance.GetChild("node", typeof (Node), new StubBuildSession()));
        }

        [Test]
        public void Fill_in_argument_by_name()
        {
            var container = new Container();
            container.SetDefault<IView, View>();

            var theNode = new Node();
            var theTrade = new Trade();

            var command = container
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
            ObjectFactory.Initialize(x =>
            {
                x.ForConcreteType<ExplicitTarget>().Configure
                    .CtorDependency<IProvider>().Is<RedProvider>()
                    .WithCtorArg("name").EqualTo("Jeremy");
            });

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            var firstTarget = ObjectFactory.GetInstance<ExplicitTarget>();
            Assert.IsInstanceOfType(typeof (RedProvider), firstTarget.Provider);

            // Now, set the explicit arg for IProvider
            var theBlueProvider = new BlueProvider();
            var secondTarget = ObjectFactory.With<IProvider>(theBlueProvider).GetInstance<ExplicitTarget>();
            Assert.AreSame(theBlueProvider, secondTarget.Provider);
        }

        [Test]
        public void NowDoItWithObjectFactoryItself_with_new_API()
        {
            ObjectFactory.Initialize(x =>
            {
                x.ForRequestedType<ExplicitTarget>().TheDefault.Is.OfConcreteType<ExplicitTarget>()
                    .CtorDependency<IProvider>().Is(child => child.OfConcreteType<RedProvider>())
                    .WithCtorArg("name").EqualTo("Jeremy");
            });

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ObjectFactory.GetInstance<ExplicitTarget>().Provider.IsType<RedProvider>();

            // Now, set the explicit arg for IProvider
            var theBlueProvider = new BlueProvider();
            ObjectFactory.With<IProvider>(theBlueProvider).GetInstance<ExplicitTarget>()
                .Provider.ShouldBeTheSameAs(theBlueProvider);
        }

        [Test]
        public void OverrideAPrimitiveWithObjectFactory()
        {
            ObjectFactory.Initialize(x =>
            {
                x.ForConcreteType<ExplicitTarget>().Configure
                    .CtorDependency<IProvider>().Is<RedProvider>()
                    .WithCtorArg("name").EqualTo("Jeremy");
            });

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            var firstTarget = ObjectFactory.GetInstance<ExplicitTarget>();
            Assert.AreEqual("Jeremy", firstTarget.Name);

            // Now, set the explicit arg for IProvider
            var secondTarget = ObjectFactory.With("name").EqualTo("Julia").GetInstance<ExplicitTarget>();
            Assert.AreEqual("Julia", secondTarget.Name);
        }

        [Test]
        public void pass_explicit_service_into_all_instances()
        {
            // The Container is constructed with 2 instances
            // of TradeView
            var container = new Container(r =>
            {
                r.ForRequestedType<TradeView>()
                    .TheDefaultIsConcreteType<TradeView>()
                    .AddConcreteType<SecuredTradeView>();
            });

            var theTrade = new Trade();

            IList<TradeView> views = container.With(theTrade).GetAllInstances<TradeView>();

            views[0].Trade.ShouldBeTheSameAs(theTrade);
            views[1].Trade.ShouldBeTheSameAs(theTrade);
        }

        [Test]
        public void Pass_in_arguments_as_dictionary()
        {
            var manager = new Container();
            manager.SetDefault<IView, View>();

            var theNode = new Node();
            var theTrade = new Trade();

            var args = new ExplicitArguments();
            args.Set(theNode);
            args.SetArg("trade", theTrade);

            var command = manager.GetInstance<Command>(args);

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

            var args = new ExplicitArguments();
            var theLump = new Lump();
            args.Set(theLump);

            var instance = (LumpProvider) manager.GetInstance<IProvider>(args);
            Assert.AreSame(theLump, instance.Lump);
        }

        [Test]
        public void PassAnArgumentIntoExplicitArgumentsForARequestedInterfaceUsingObjectFactory()
        {
            ObjectFactory.Initialize(x => { x.ForRequestedType<IProvider>().TheDefaultIsConcreteType<LumpProvider>(); });


            var theLump = new Lump();

            var provider = (LumpProvider) ObjectFactory.With(theLump).GetInstance<IProvider>();
            Assert.AreSame(theLump, provider.Lump);
        }

        [Test]
        public void PassAnArgumentIntoExplicitArgumentsThatMightNotAlreadyBeRegistered()
        {
            var theLump = new Lump();
            var provider = ObjectFactory.With(theLump).GetInstance<LumpProvider>();
            Assert.AreSame(theLump, provider.Lump);
        }

        [Test]
        public void PassExplicitArgsIntoInstanceManager()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ExplicitTarget>().Configure
                    .CtorDependency<IProvider>().Is<RedProvider>()
                    .WithCtorArg("name").EqualTo("Jeremy");
            });

            var args = new ExplicitArguments();

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            var firstTarget = container.GetInstance<ExplicitTarget>(args);
            Assert.IsInstanceOfType(typeof (RedProvider), firstTarget.Provider);

            // Now, set the explicit arg for IProvider
            args.Set<IProvider>(new BlueProvider());
            var secondTarget = container.GetInstance<ExplicitTarget>(args);
            Assert.IsInstanceOfType(typeof (BlueProvider), secondTarget.Provider);
        }

        [Test]
        public void RegisterAndFindServicesOnTheExplicitArgument()
        {
            var args = new ExplicitArguments();
            Assert.IsNull(args.Get<IProvider>());

            var red = new RedProvider();
            args.Set<IProvider>(red);

            Assert.AreSame(red, args.Get<IProvider>());

            args.Set<IExplicitTarget>(new RedTarget());
            Assert.IsInstanceOfType(typeof (RedTarget), args.Get<IExplicitTarget>());
        }

        [Test]
        public void RegisterAndRetrieveArgs()
        {
            var args = new ExplicitArguments();
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

    public class TradeView : IView
    {
        private readonly Trade _trade;

        public TradeView(Trade trade)
        {
            _trade = trade;
        }

        public Trade Trade
        {
            get { return _trade; }
        }
    }

    public class SecuredTradeView : TradeView
    {
        public SecuredTradeView(Trade trade) : base(trade)
        {
        }
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

    public class TradeNode : Node
    {
        private readonly Trade _trade;

        public TradeNode(Trade trade)
        {
            _trade = trade;
        }

        public Trade Trade
        {
            get { return _trade; }
        }
    }
}