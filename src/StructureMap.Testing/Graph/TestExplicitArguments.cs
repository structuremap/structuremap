using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class TestExplicitArguments
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            ObjectFactory.Initialize(x => { x.UseDefaultStructureMapConfigFile = false; });
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


            public string Name { get { return _name; } }

            public IProvider Provider { get { return _provider; } }
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

        public class ClassWithNoArgs
        {
            public Address TheAddress { get; set; }
        }

        public class Address
        {
        }

        public class SpecialInstance : Instance
        {
            protected override string getDescription()
            {
                return string.Empty;
            }

            protected override object build(Type pluginType, BuildSession session)
            {
                return new ClassWithNoArgs
                {
                    TheAddress = (Address) session.CreateInstance(typeof (Address))
                };
            }
        }

        public class SpecialNode : Node
        {
        }

        [Test]
        public void can_build_a_concrete_class_with_constructor_args_that_is_not_previously_registered()
        {
            var container = new Container();
            container.With("name").EqualTo("Jeremy").GetInstance<ConcreteThatNeedsString>()
                .Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void can_build_a_concrete_class_with_constructor_args_that_is_not_previously_registered_2()
        {
            var container = new Container();

            container.With(x => { x.With("name").EqualTo("Jeremy"); }).GetInstance<ConcreteThatNeedsString>().Name.
                ShouldEqual("Jeremy");
        }

        [Test]
        public void can_build_a_concrete_type_from_explicit_args_passed_into_a_named_instance()
        {
            var container = new Container(x =>
            {
                x.For<ColorWithLump>().AddInstances(o =>
                {
                    o.Type<ColorWithLump>().WithCtorArg("color").EqualTo("red").WithName("red");
                    o.Type<ColorWithLump>().WithCtorArg("color").EqualTo("green").WithName("green");
                    o.Type<ColorWithLump>().WithCtorArg("color").EqualTo("blue").WithName("blue");
                });
            });


            var lump = new Lump();

            var colorLump = container.With(lump).GetInstance<ColorWithLump>("red");
            colorLump.Lump.ShouldBeTheSameAs(lump);
            colorLump.Color.ShouldEqual("red");
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
                r.For<IView>().Use<TradeView>();
                r.For<Node>().Use<TradeNode>();
            });

            var command = container.With(theTrade).GetInstance<Command>();

            command.Trade.ShouldBeTheSameAs(theTrade);
            command.Node.IsType<TradeNode>().Trade.ShouldBeTheSameAs(theTrade);
            command.View.IsType<TradeView>().Trade.ShouldBeTheSameAs(theTrade);
        }

        [Test]
        public void ExplicitArguments_can_return_child_by_name()
        {
            var theNode = new Node();
            var container = new Container(x => { x.For<IView>().Use<View>(); });
            container.With("node").EqualTo(theNode).GetInstance<Command>().Node.ShouldBeTheSameAs(theNode);
        }

        [Test]
        public void Fill_in_argument_by_name()
        {
            var container = new Container(x => { x.For<IView>().Use<View>(); });

            var theNode = new Node();
            var theTrade = new Trade();

            var command = container
                .With("node").EqualTo(theNode)
                .With(theTrade)
                .GetInstance<Command>();

            command.View.ShouldBeOfType<View>();
            theNode.ShouldBeTheSameAs(command.Node);
            theTrade.ShouldBeTheSameAs(command.Trade);
        }

        [Test]
        public void Fill_in_argument_by_type()
        {
            var container = new Container(x => { x.For<IView>().Use<View>(); });

            var theNode = new SpecialNode();
            var theTrade = new Trade();

            var command = container
                .With(typeof (Node), theNode)
                .With(theTrade)
                .GetInstance<Command>();

            command.View.ShouldBeOfType<View>();
            theNode.ShouldBeTheSameAs(command.Node);
            theTrade.ShouldBeTheSameAs(command.Trade);
        }

        [Test]
        public void Fill_in_argument_by_type_with_ObjectFactory()
        {
            ObjectFactory.Initialize(x => { x.For<IView>().Use<View>(); });

            var theNode = new SpecialNode();
            var theTrade = new Trade();

            var command = ObjectFactory
                .With(typeof (Node), theNode)
                .With(theTrade)
                .GetInstance<Command>();

            command.View.ShouldBeOfType<View>();
            theNode.ShouldBeTheSameAs(command.Node);
            theTrade.ShouldBeTheSameAs(command.Trade);
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
            firstTarget.Provider.ShouldBeOfType<RedProvider>();

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
                x.For<ExplicitTarget>().Use<ExplicitTarget>()
                    .CtorDependency<IProvider>().Is(child => child.Type<RedProvider>())
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
                r.For<TradeView>().Use<TradeView>();
                r.For<TradeView>().Add<SecuredTradeView>();
            });

            var theTrade = new Trade();
            IList<TradeView> views = container.With(theTrade).GetAllInstances<TradeView>();

            views[0].Trade.ShouldBeTheSameAs(theTrade);
            views[1].Trade.ShouldBeTheSameAs(theTrade);
        }

        [Test]
        public void pass_explicit_service_into_all_instances_and_retrieve_without_generics()
        {
            // The Container is constructed with 2 instances
            // of TradeView
            var container = new Container(r =>
            {
                r.For<TradeView>().Use<TradeView>();
                r.For<TradeView>().AddConcreteType<SecuredTradeView>();
            });

            var theTrade = new Trade();

            IList views = container.With(theTrade).GetAllInstances(typeof (TradeView));

            ((TradeView) views[0]).Trade.ShouldBeTheSameAs(theTrade);
            ((TradeView) views[1]).Trade.ShouldBeTheSameAs(theTrade);
        }


        [Test]
        public void Pass_in_arguments_as_dictionary()
        {
            var container = new Container(x => { x.For<IView>().Use<View>(); });

            var theNode = new Node();
            var theTrade = new Trade();

            var args = new ExplicitArguments();
            args.Set(theNode);
            args.SetArg("trade", theTrade);

            var command = container.GetInstance<Command>(args);

            command.View.ShouldBeOfType<View>();
            theNode.ShouldBeTheSameAs(command.Node);
            theTrade.ShouldBeTheSameAs(command.Trade);
        }


        [Test]
        public void PassAnArgumentIntoExplicitArgumentsForARequestedInterface()
        {
            IContainer manager =
                new Container(
                    registry => registry.For<IProvider>().Use<LumpProvider>());

            var args = new ExplicitArguments();
            var theLump = new Lump();
            args.Set(theLump);

            var instance = (LumpProvider) manager.GetInstance<IProvider>(args);
            Assert.AreSame(theLump, instance.Lump);
        }

        [Test]
        public void PassAnArgumentIntoExplicitArgumentsForARequestedInterfaceUsingObjectFactory()
        {
            ObjectFactory.Initialize(x => { x.For<IProvider>().Use<LumpProvider>(); });


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
            firstTarget.Provider.ShouldBeOfType<RedProvider>();

            // Now, set the explicit arg for IProvider
            args.Set<IProvider>(new BlueProvider());
            var secondTarget = container.GetInstance<ExplicitTarget>(args);
            secondTarget.Provider.ShouldBeOfType<BlueProvider>();
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
            args.Get<IExplicitTarget>().ShouldBeOfType<RedTarget>();
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

        [Test]
        public void use_a_type_that_is_not_part_of_the_constructor_in_the_with()
        {
            var container = new Container();
            container.With(new Address()).GetInstance<ClassWithNoArgs>()
                .ShouldBeOfType<ClassWithNoArgs>();
        }

        [Test]
        public void use_explicit_type_arguments_with_custom_instance()
        {
            var container =
                new Container(x => { x.For<ClassWithNoArgs>().Use(new SpecialInstance()); });

            Debug.WriteLine(container.WhatDoIHave());

            var address = new Address();


            container.With(address).GetInstance<ClassWithNoArgs>()
                .TheAddress.ShouldBeTheSameAs(address);
        }
    }

    public class Lump
    {
    }

    public class ColorWithLump
    {
        private readonly string _color;
        private readonly Lump _lump;

        public ColorWithLump(string color, Lump lump)
        {
            _color = color;
            _lump = lump;
        }

        public string Color { get { return _color; } }

        public Lump Lump { get { return _lump; } }
    }

    public class LumpProvider : TestExplicitArguments.IProvider
    {
        private readonly Lump _lump;

        public LumpProvider(Lump lump)
        {
            _lump = lump;
        }


        public Lump Lump { get { return _lump; } }
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

        public Trade Trade { get { return _trade; } }
    }

    public class SecuredTradeView : TradeView
    {
        public SecuredTradeView(Trade trade)
            : base(trade)
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

        public Trade Trade { get { return _trade; } }

        public Node Node { get { return _node; } }

        public IView View { get { return _view; } }
    }

    public class TradeNode : Node
    {
        private readonly Trade _trade;

        public TradeNode(Trade trade)
        {
            _trade = trade;
        }

        public Trade Trade { get { return _trade; } }
    }

    public class ConcreteThatNeedsString
    {
        private readonly string _name;

        public ConcreteThatNeedsString(string name)
        {
            _name = name;
        }

        public string Name { get { return _name; } }
    }
}