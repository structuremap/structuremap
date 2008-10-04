using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    public class ClassWithOneSetterBuilder : InstanceBuilder
    {
        public override Type PluggedType
        {
            get { throw new System.NotImplementedException(); }
        }

        public override object BuildInstance(IConfiguredInstance instance, BuildSession session)
        {
            ClassWithOneSetter target = new ClassWithOneSetter();
            if (instance.HasProperty("Name")) target.Name = instance.GetProperty("Name");

            return target;
        }
    }


    [TestFixture]
    public class OptionalSetterInjectionTester
    {
        [SetUp]
        public void SetUp()
        {
            PluginCache.ResetAll();
        }

        [Test]
        public void optional_setter_injection_with_string()
        {
            var container = new Container(
                r =>
                {
                    r.InstanceOf<OptionalSetterTarget>().Is.OfConcreteType<OptionalSetterTarget>().WithName("NoName");
                       
                    r.ForConcreteType<OptionalSetterTarget>().Configure
                        .WithProperty("Name").EqualTo("Jeremy");
                });

            try
            {
                //container.GetInstance<NoSettersTarget>();
                container.GetInstance<OptionalSetterTarget>().Name.ShouldEqual("Jeremy");
                container.GetInstance<OptionalSetterTarget>("NoName").Name.ShouldBeNull();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Test]
        public void one_optional_setter_injection_with_string()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ClassWithOneSetter>().Configure
                    .WithProperty("Name").EqualTo("Jeremy");
            });

            container.GetInstance<ClassWithOneSetter>().Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void one_optional_enum_setter()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ClassWithOneEnum>().Configure
                    .WithProperty("Color").EqualTo("Red");
            });

            container.GetInstance<ClassWithOneEnum>().Color.ShouldEqual(ColorEnum.Red);
        }

        [Test]
        public void one_optional_long_and_one_bool_setter()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ClassWithOneLongAndOneBool>().Configure
                    .WithProperty("Age").EqualTo(34)
                    .WithProperty("Active").EqualTo(true);
            });

            var instance = container.GetInstance<ClassWithOneLongAndOneBool>();
            instance.Age.ShouldEqual(34);
            instance.Active.ShouldBeTrue();
        }

        [Test]
        public void one_optional_child_setter_with_the_setter_property_defined()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ClassWithDependency>().Configure
                    .SetterDependency<Rule>().Is(new ColorRule("Red"));
            });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof(ColorRule));
        }


        [Test]
        public void one_optional_child_setter_without_the_setter_property_defined()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ClassWithDependency>();
            });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeNull();
        }

        [Test]
        public void read_instance_from_xml_with_optional_setter_defined()
        {
            Debug.WriteLine(typeof(ClassWithDependency).AssemblyQualifiedName);

            var graph = DataMother.BuildPluginGraphFromXml(@"
<StructureMap MementoStyle='Attribute'>
    <DefaultInstance 
        PluginType='StructureMap.Testing.Pipeline.ClassWithDependency, StructureMap.Testing' 
        PluggedType='StructureMap.Testing.Pipeline.ClassWithDependency, StructureMap.Testing'>
        
        <Rule PluggedType='StructureMap.Testing.Widget.ColorRule, StructureMap.Testing.Widget' color='Red' />
    </DefaultInstance>
</StructureMap>

");

            Container container = new Container(graph);

            container.GetInstance<ClassWithDependency>().Rule.IsType<ColorRule>().Color.ShouldEqual("Red");
        }



        [Test]
        public void read_instance_from_xml_with_optional_setter_not_defined()
        {
            Debug.WriteLine(typeof(ClassWithDependency).AssemblyQualifiedName);

            var graph = DataMother.BuildPluginGraphFromXml(@"
<StructureMap MementoStyle='Attribute'>
    <DefaultInstance 
        PluginType='StructureMap.Testing.Pipeline.ClassWithDependency, StructureMap.Testing' 
        PluggedType='StructureMap.Testing.Pipeline.ClassWithDependency, StructureMap.Testing'>

    </DefaultInstance>
</StructureMap>

");

            Container container = new Container(graph);

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeNull();
        }




        [Test]
        public void one_optional_child_setter2()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ClassWithDependency>().Configure
                    .SetterDependency<Rule>().Is(new ColorRule("Red"));

            });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof(ColorRule));
        }

        [Test]
        public void using_the_FillAllPropertiesOf()
        {
            var container =
                new Container(
                    r =>
                    r.FillAllPropertiesOfType<Rule>().TheDefault.Is.Object(new ColorRule("Red")));

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof(ColorRule));
        }

        [Test]
        public void one_optional_child_array_setter()
        {
            var container = new Container(x =>
            {
                x.ForRequestedType<ClassWithDependency>().TheDefault.Is.OfConcreteType<ClassWithDependency>()
                    .TheArrayOf<Rule>().Contains(arr =>
                    {
                        arr.IsThis(new ColorRule("Red"));
                    });
            });

            container.GetInstance<ClassWithDependency>().Rules.Length.ShouldEqual(1);
        }

        [Test]
        public void AutoFill_a_property()
        {
            var container = new Container(r =>
            {
                r.ForConcreteType<ClassWithDependency>().Configure
                    .SetterDependency<Rule>().IsTheDefault();

                r.ForRequestedType<Rule>().TheDefault.Is.Object(new ColorRule("Green"));
            });

                      

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof(ColorRule));
        }

        private static Logger createLogger(IContext session)
        {
            return new Logger(session.ParentType);
        }

        [Test]
        public void AutoFill_a_property_with_contextual_construction()
        {
            var container =
                new Container(
                    r =>
                    r.FillAllPropertiesOfType<Logger>().TheDefault.Is.ConstructedBy(createLogger));

            container.GetInstance<ClassWithLogger>().Logger.Type.ShouldEqual(typeof (ClassWithLogger));
            container.GetInstance<ClassWithLogger2>().Logger.Type.ShouldEqual(typeof (ClassWithLogger2));

            container.GetInstance<ClassWithClassWithLogger>().ClassWithLogger.Logger.Type.ShouldEqual(
                typeof (ClassWithLogger));
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

    public class Logger
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
    }

    public class ClassWithLogger
    {
        public Logger Logger { get; set; }
    }

    public class ClassWithLogger2
    {
        public Logger Logger { get; set; }
    }

    public enum ColorEnum
    {
        Red, Blue, Green
    }

    public class OptionalSetterTarget
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        private string _name2;
        public string Name2
        {
            get { return _name2; }
            set
            {
                _name2 = value;
            }
        }
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

    public class ClassWithOneEnumBuilder : InstanceBuilder
    {
        public override Type PluggedType
        {
            get { throw new System.NotImplementedException(); }
        }

        public override object BuildInstance(IConfiguredInstance instance, BuildSession session)
        {
            ClassWithOneEnum target = new ClassWithOneEnum();
            if (instance.HasProperty("Color")) target.Color = (ColorEnum) Enum.Parse(typeof (ColorEnum), instance.GetProperty("Color"));

            return target;
        }
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