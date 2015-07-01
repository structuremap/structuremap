using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Testing.Samples;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class inline_dependencies
    {
        // SAMPLE: inline-dependencies-ColorWidget
        public class ColorWidget : IWidget
        {
            public string Color { get; set; }

            public ColorWidget(string color)
            {
                Color = color;
            }
        }
        // ENDSAMPLE

        // SAMPLE: inline-dependencies-value
        [Test]
        public void inline_usage_of_primitive_constructor_argument()
        {
            var container = new Container(_ => {
                _.For<IWidget>().Use<ColorWidget>()
                    .Ctor<string>().Is("Red");
            });

            container.GetInstance<IWidget>()
                .ShouldBeOfType<ColorWidget>()
                .Color.ShouldBe("Red");
        }
        // ENDSAMPLE

        // SAMPLE: inline-dependencies-rule-classes
        public class SomeEvent{}

        public interface ICondition
        {
            bool Matches(SomeEvent @event);
        }

        public interface IAction
        {
            void PerformWork(SomeEvent @event);
        }

        public interface IEventRule
        {
            void ProcessEvent(SomeEvent @event);
        }
        // ENDSAMPLE

        // SAMPLE: inline-dependencies-SimpleRule
        public class SimpleRule : IEventRule
        {
            private readonly ICondition _condition;
            private readonly IAction _action;

            public SimpleRule(ICondition condition, IAction action)
            {
                _condition = condition;
                _action = action;
            }

            public void ProcessEvent(SomeEvent @event)
            {
                if (_condition.Matches(@event))
                {
                    _action.PerformWork(@event);
                }
            }
        }
        // ENDSAMPLE

        public class Condition1 : ICondition
        {
            public bool Matches(SomeEvent @event)
            {
                return true;
            }
        }

        public class Condition2 : Condition1{}
        public class Condition3 : Condition1{}

        public class Action1 : IAction
        {
            public void PerformWork(SomeEvent @event)
            {
                throw new System.NotImplementedException();
            }
        }

        public class Action2 : Action1{}
        public class Action3 : Action1{}


        // SAMPLE: inline-dependencies-simple-ctor-injection
        public class InlineCtorArgs : Registry
        {
            public InlineCtorArgs()
            {
                // Defining args by type
                For<IEventRule>().Use<SimpleRule>()
                    .Ctor<ICondition>().Is<Condition1>()
                    .Ctor<IAction>().Is<Action1>()
                    .Named("One");

                // Pass the explicit values for dependencies
                For<IEventRule>().Use<SimpleRule>()
                    .Ctor<ICondition>().Is(new Condition2())
                    .Ctor<IAction>().Is(new Action2())
                    .Named("Two");

                // Use Lambda construction
                For<IEventRule>().Use<SimpleRule>()
                    .Ctor<ICondition>().Is(() => new Condition3())
                    .Ctor<IAction>().Is("some crazy builder", c => c.GetInstance<Action3>())
                    .Named("Three");

                // Rarely used, but gives you a "do any crazy thing" option
                // Pass in your own Instance object
                For<IEventRule>().Use<SimpleRule>()
                    .Ctor<IAction>().Is(new MySpecialActionInstance());

                // Inline configuration of your dependency's dependencies

                For<IEventRule>().Use<SimpleRule>()
                    .Ctor<ICondition>().IsSpecial(_ => _.Type<BigCondition>().Ctor<int>().Is(100))

                    // or

                    .Ctor<ICondition>().Is(new SmartInstance<BigCondition>().Ctor<int>().Is(100));
            }

            public class BigCondition : ICondition
            {
                public BigCondition(int number)
                {
                }

                public bool Matches(SomeEvent @event)
                {
                    throw new NotImplementedException();
                }
            }

            public class MySpecialActionInstance : LambdaInstance<Action3>
            {
                public MySpecialActionInstance()
                    : base(() => new Action3())
                {
                }
            }
        }
        // ENDSAMPLE

        // SAMPLE: inline-dependencies-ctor-by-name
        public class DualConditionRule : IEventRule
        {
            private readonly ICondition _first;
            private readonly ICondition _second;
            private readonly IAction _action;

            public DualConditionRule(ICondition first, ICondition second, IAction action)
            {
                _first = first;
                _second = second;
                _action = action;
            }

            public void ProcessEvent(SomeEvent @event)
            {
                if (_first.Matches(@event) || _second.Matches(@event))
                {
                    _action.PerformWork(@event);
                }
            }
        }

        public class DualConditionRuleRegistry : Registry
        {
            public DualConditionRuleRegistry()
            {
                // In this case, because DualConditionRule
                // has two different 
                For<IEventRule>().Use<DualConditionRule>()
                    .Ctor<ICondition>("first").Is<Condition1>()
                    .Ctor<ICondition>("second").Is<Condition2>();
            }
        }
        // ENDSAMPLE



        // SAMPLE: inline-dependencies-setters
        public class RuleWithSetters : IEventRule
        {
            public ICondition Condition { get; set; }
            public IAction Action { get; set; }

            public void ProcessEvent(SomeEvent @event)
            {
                if (Condition.Matches(@event))
                {
                    Action.PerformWork(@event);
                }
            }
        }

        public class RuleWithSettersRegistry : Registry
        {
            public RuleWithSettersRegistry()
            {
                For<IEventRule>().Use<RuleWithSetters>()
                    .Setter<ICondition>().Is<Condition1>()

                    // or

                    .Setter(x => x.Action).Is(new Action1())

                    // or if you need to specify the name

                    .Setter<IAction>("Action").Is<Action2>()

                    // or you can configure values *after* the object
                    // is constructed with the SetProperty method

                    .SetProperty(x => x.Action = new Action2());
            }
        }
        // ENDSAMPLE

        // SAMPLE: inline-dependencies-open-types
        public interface IEventRule<TEvent>
        {
            void ProcessEvent(TEvent @event);
        }

        public interface ICondition<TEvent>
        {
            bool Matches(TEvent @event);
        }

        public class Condition1<TEvent> : ICondition<TEvent>
        {
            public bool Matches(TEvent @event)
            {
                throw new NotImplementedException();
            }
        }

        public interface IAction<TEvent>
        {
            void PerformWork(TEvent @event);
        }

        public class Action1<TEvent> : IAction<TEvent>
        {
            public void PerformWork(TEvent @event)
            {
                throw new NotImplementedException();
            }
        }

        public class EventRule<TEvent> : IEventRule<TEvent>
        {
            private readonly string _name;
            private readonly ICondition<TEvent> _condition;
            private readonly IAction<TEvent> _action;

            public EventRule(string name, ICondition<TEvent> condition, IAction<TEvent> action)
            {
                _name = name;
                _condition = condition;
                _action = action;
            }

            public string Name
            {
                get { return _name; }
            }

            public void ProcessEvent(TEvent @event)
            {
                if (_condition.Matches(@event))
                {
                    _action.PerformWork(@event);
                }
            }
        }
        // ENDSAMPLE

        // SAMPLE: inline-dependencies-programmatic-configuration
        public class OpenTypesRegistry : Registry
        {
            public OpenTypesRegistry()
            {
                var instance = new ConstructorInstance(typeof (EventRule<>));

                // By name
                instance.Dependencies.Add("action", typeof(Action1<>) );

                // Everything else is syntactical sugur over this:
                instance.Dependencies.Add(new Argument
                {
                    Type = typeof(IAction<>), // The dependency type
                    Name = "action",          // The name of the dependency, either
                                              // a constructor argument name or
                                              // the name of a setter property

                    // Specify the actual dependency
                    // This can be either a concrete type, the prebuilt value,
                    // or an Instance
                    Dependency = typeof(Action1<>)
                                              
                });
            }
        }
        // ENDSAMPLE

        // SAMPLE: inline-dependencies-enumerables
        public class BigRule : IEventRule
        {
            private readonly IEnumerable<ICondition> _conditions;
            private readonly IEnumerable<IAction> _actions;

            public BigRule(IEnumerable<ICondition> conditions, IEnumerable<IAction> actions)
            {
                _conditions = conditions;
                _actions = actions;
            }

            public void ProcessEvent(SomeEvent @event)
            {
                if (_conditions.Any(x => x.Matches(@event)))
                {
                    _actions.Each(x => x.PerformWork(@event));
                }
            }
        }

        public class BigRuleRegistry : Registry
        {
            public BigRuleRegistry()
            {
                For<IEventRule>().Use<BigRule>()

                    // Each line in the nested closure adds another
                    // ICondition to the enumerable dependency in
                    // the order in which they are configured
                    .EnumerableOf<ICondition>().Contains(_ => {
                        _.Type<Condition1>();
                        _.Type<Condition2>();
                    })
                    .EnumerableOf<IAction>().Contains(_ => {
                        _.Type<Action1>();
                        _.Object(new Action2());
                    });
            }
        }
        // ENDSAMPLE
    }
}