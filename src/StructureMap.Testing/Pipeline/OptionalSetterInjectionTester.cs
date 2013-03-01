using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class OptionalSetterInjectionTester
    {
        [SetUp]
        public void SetUp()
        {
            PluginCache.ResetAll();
        }

        private static Logger createLogger(IContext session)
        {
            return new Logger(session.ParentType);
        }

        [Test]
        public void AutoFill_a_property()
        {
            var container = new Container(r => {
                r.ForConcreteType<ClassWithDependency>().Configure
                 .Setter<Rule>().IsTheDefault();

                r.For<Rule>().Use(new ColorRule("Green"));
            });


            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof (ColorRule));
        }

        [Test]
        public void AutoFill_a_property_with_contextual_construction()
        {
            var container =
                new Container(
                    r => { r.FillAllPropertiesOfType<ILogger>().Use(context => new Logger(context.ParentType)); });

            container.GetInstance<ClassWithLogger>().Logger.ShouldBeOfType<Logger>().Type.ShouldEqual(
                typeof (ClassWithLogger));
            container.GetInstance<ClassWithLogger2>().Logger.ShouldBeOfType<Logger>().Type.ShouldEqual(
                typeof (ClassWithLogger2));

            container.GetInstance<ClassWithClassWithLogger>().ClassWithLogger.Logger.ShouldBeOfType<Logger>().Type.
                      ShouldEqual(
                          typeof (ClassWithLogger));
        }

        [Test]
        public void one_optional_child_array_setter()
        {
            var container = new Container(x => {
                x.For<ClassWithDependency>().Use<ClassWithDependency>()
                 .EnumerableOf<Rule>().Contains(arr => { arr.IsThis(new ColorRule("Red")); });
            });

            container.GetInstance<ClassWithDependency>().Rules.Length.ShouldEqual(1);
        }

        [Test]
        public void one_optional_child_setter2()
        {
            var container = new Container(r => {
                r.ForConcreteType<ClassWithDependency>().Configure
                 .Setter<Rule>().Is(new ColorRule("Red"));
            });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof (ColorRule));
        }

        [Test]
        public void one_optional_child_setter_with_the_setter_property_defined()
        {
            var container = new Container(r => {
                r.ForConcreteType<ClassWithDependency>().Configure
                 .Setter<Rule>().Is(new ColorRule("Red"));
            });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof (ColorRule));
        }


        [Test]
        public void one_optional_child_setter_without_the_setter_property_defined()
        {
            var container = new Container(r => { r.ForConcreteType<ClassWithDependency>(); });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeNull();
        }

        [Test]
        public void one_optional_enum_setter()
        {
            var container = new Container(r => {
                r.ForConcreteType<ClassWithOneEnum>().Configure
                 .Setter(x => x.Color).Is(ColorEnum.Red);
            });

            container.GetInstance<ClassWithOneEnum>().Color.ShouldEqual(ColorEnum.Red);
        }

        [Test]
        public void one_optional_long_and_one_bool_setter()
        {
            var container = new Container(r => {
                r.ForConcreteType<ClassWithOneLongAndOneBool>().Configure
                 .Setter(x => x.Age).Is(34)
                 .Setter(x => x.Active).Is(true);
            });

            var instance = container.GetInstance<ClassWithOneLongAndOneBool>();
            instance.Age.ShouldEqual(34);
            instance.Active.ShouldBeTrue();
        }

        [Test]
        public void one_optional_setter_injection_with_string()
        {
            var container = new Container(r => {
                r.ForConcreteType<ClassWithOneSetter>().Configure
                 .Setter(x => x.Name).Is("Jeremy");
            });

            container.GetInstance<ClassWithOneSetter>().Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void optional_setter_injection_with_string()
        {
            var container = new Container(r => {
                // The "Name" property is not configured for this instance
                r.For<OptionalSetterTarget>().Use<OptionalSetterTarget>().Named("NoName");

                // The "Name" property is configured for this instance
                r.ForConcreteType<OptionalSetterTarget>().Configure
                 .Setter(x => x.Name).Is("Jeremy");
            });

            container.GetInstance<OptionalSetterTarget>().Name.ShouldEqual("Jeremy");
            container.GetInstance<OptionalSetterTarget>("NoName").Name.ShouldBeNull();
        }

        [Test]
        public void optional_setter_with_Action()
        {
            var container = new Container(r => {
                // The "Name" property is not configured for this instance
                r.For<OptionalSetterTarget>().Use<OptionalSetterTarget>().Named("NoName");

                // The "Name" property is configured for this instance
                r.ForConcreteType<OptionalSetterTarget>().Configure
                 .Setter(x => x.Name).Is("Jeremy");
            });

            container.GetInstance<OptionalSetterTarget>().Name.ShouldEqual("Jeremy");
            container.GetInstance<OptionalSetterTarget>("NoName").Name.ShouldBeNull();
        }

        [Test]
        public void using_the_FillAllPropertiesOf()
        {
            var container =
                new Container(
                    r =>
                    r.FillAllPropertiesOfType<Rule>().Use(new ColorRule("Red")));

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof (ColorRule));
        }
    }

    public class ClassWithClassWithLogger
    {
        private readonly ClassWithLogger _classWithLogger;

        public ClassWithClassWithLogger(ClassWithLogger classWithLogger)
        {
            _classWithLogger = classWithLogger;
        }

        public ClassWithLogger ClassWithLogger
        {
            get { return _classWithLogger; }
        }
    }

    public interface ILogger
    {
        void LogMessage(string message);
    }

    public class Logger : ILogger
    {
        private readonly Type _type;

        public Logger(Type type)
        {
            _type = type;
        }

        public Type Type
        {
            get { return _type; }
        }

        public void LogMessage(string message)
        {
        }
    }

    public class ClassWithLogger
    {
        public ILogger Logger { get; set; }
    }

    public class ClassWithLogger2
    {
        public ILogger Logger { get; set; }
    }

    public enum ColorEnum
    {
        Red,
        Blue,
        Green
    }

    public class OptionalSetterTarget
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
    }

    public class NoSettersTarget
    {
    }

    public class ClassWithOneSetter
    {
        public string Name { get; set; }
    }

    public class ClassWithOneEnum
    {
        public ColorEnum Color { get; set; }
    }

    public class ClassWithOneLongAndOneBool
    {
        public bool Active { get; set; }
        public int Age { get; set; }
    }

    public class ClassWithDependency
    {
        public Rule Rule { get; set; }
        public Rule[] Rules { get; set; }
    }
}