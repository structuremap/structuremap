using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class DeepInstanceTester
    {
        private Thing _prototype = new Thing(4, "Jeremy", .333, new WidgetRule(new ColorWidget("yellow")));

        private void assertThingMatches(Registry registry)
        {
            IInstanceManager manager = registry.BuildInstanceManager();
            Thing actual = manager.CreateInstance<Thing>();
            Assert.AreEqual(_prototype, actual);
        }

        [Test]
        public void DeepInstanceTest1()
        {
            Registry registry = new Registry();
            InstanceExpression widgetExpression = Registry.Instance<IWidget>()
                .UsingConcreteType<ColorWidget>()
                .WithProperty("Color").EqualTo("yellow");

            InstanceExpression ruleExpression = Registry.Instance<Rule>()
                .UsingConcreteType<WidgetRule>()
                .Child<IWidget>().Is(widgetExpression);

            registry.BuildInstancesOf<Thing>().TheDefaultIs(
                Registry.Instance<Thing>()
                    .UsingConcreteType<Thing>()
                    .WithProperty("name").EqualTo("Jeremy")
                    .WithProperty("count").EqualTo(4)
                    .WithProperty("average").EqualTo(.333)
                    .Child<Rule>().Is(
                        ruleExpression
                    )
                );

            assertThingMatches(registry);
        }

        [Test]
        public void DeepInstance2()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<IWidget>().TheDefaultIs(
                Registry.Instance<IWidget>().UsingConcreteType<ColorWidget>()
                    .WithProperty("Color").EqualTo("yellow")
                );

            registry.BuildInstancesOf<Rule>().TheDefaultIsConcreteType<WidgetRule>();

            registry.BuildInstancesOf<Thing>().TheDefaultIs(
                Registry.Instance<Thing>()
                    .UsingConcreteType<Thing>()
                    .WithProperty("average").EqualTo(.333)
                    .WithProperty("name").EqualTo("Jeremy")
                    .WithProperty("count").EqualTo(4)
                );

            assertThingMatches(registry);
        }

        [Test]
        public void DeepInstance3()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<IWidget>().TheDefaultIs(
                Registry.Object(new ColorWidget("yellow"))    
                );

            registry.BuildInstancesOf<Rule>().TheDefaultIsConcreteType<WidgetRule>();

            registry.BuildInstancesOf<Thing>().TheDefaultIs(
                Registry.Instance<Thing>()
                    .UsingConcreteType<Thing>()
                    .WithProperty("average").EqualTo(.333)
                    .WithProperty("name").EqualTo("Jeremy")
                    .WithProperty("count").EqualTo(4)
                );

            assertThingMatches(registry);
        }


        [Test]
        public void DeepInstance4()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<IWidget>().TheDefaultIs(
                Registry.Prototype(new ColorWidget("yellow"))
                );

            registry.BuildInstancesOf<Rule>().TheDefaultIsConcreteType<WidgetRule>();

            registry.BuildInstancesOf<Thing>().TheDefaultIs(
                Registry.Instance<Thing>()
                    .UsingConcreteType<Thing>()
                    .WithProperty("average").EqualTo(.333)
                    .WithProperty("name").EqualTo("Jeremy")
                    .WithProperty("count").EqualTo(4)
                );

            assertThingMatches(registry);
        }



        [Test]
        public void DeepInstance5()
        {
            Registry registry = new Registry();

            registry.AddInstanceOf<IWidget>()
                .UsingConcreteType<ColorWidget>()
                .WithName("Yellow")
                .WithProperty("Color").EqualTo("yellow");

            registry.AddInstanceOf<Rule>()
                .UsingConcreteType<WidgetRule>()
                .WithName("TheWidgetRule")
                .Child<IWidget>().IsNamedInstance("Yellow");

            registry.BuildInstancesOf<Thing>().TheDefaultIs(
                Registry.Instance<Thing>()
                    .UsingConcreteType<Thing>()
                    .WithProperty("average").EqualTo(.333)
                    .WithProperty("name").EqualTo("Jeremy")
                    .WithProperty("count").EqualTo(4)
                    .Child<Rule>().IsNamedInstance("TheWidgetRule")
                );

            assertThingMatches(registry);
        }

    }

    public class Thing
    {
        private int _count;
        private string _name;
        private double _average;
        private Rule _rule;


        public Thing(int count, string name, double average, Rule rule)
        {
            _count = count;
            _name = name;
            _average = average;
            _rule = rule;
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Thing thing = obj as Thing;
            if (thing == null) return false;
            if (_count != thing._count) return false;
            if (!Equals(_name, thing._name)) return false;
            if (_average != thing._average) return false;
            if (!Equals(_rule, thing._rule)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            int result = _count;
            result = 29*result + (_name != null ? _name.GetHashCode() : 0);
            result = 29*result + _average.GetHashCode();
            result = 29*result + (_rule != null ? _rule.GetHashCode() : 0);
            return result;
        }
    }
}
