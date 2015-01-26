using System;
using System.Linq;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConstructorInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void to_dependency_source()
        {
            var instance = ConstructorInstance.For<StubbedGateway>();
            var source = instance.ToDependencySource(typeof (IGateway))
                .ShouldBeOfType<LifecycleDependencySource>();

            source.Instance.ShouldEqual(instance);
            source.PluginType.ShouldEqual(typeof (IGateway));
        }

        [Test]
        public void set_and_get_a_collection()
        {
            var container = new Container(x => {
                x.ForConcreteType<ClassWithArrayOfWidgets>().Configure.EnumerableOf<IWidget>()
                    .Contains(
                        new SmartInstance<ColorWidget>().Ctor<string>("color").Is("red"),
                        new SmartInstance<ColorWidget>().Ctor<string>("color").Is("green"),
                        new ObjectInstance(new AWidget())
                    );
            });

            var widgets = container.GetInstance<ClassWithArrayOfWidgets>()
                .Widgets;

            widgets.Length.ShouldEqual(3);

            widgets[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
            widgets[2].ShouldBeOfType<AWidget>();
        }

        [Test]
        public void constructor_instance_picks_up_InstanceAttributes()
        {
            var instance = new ConstructorInstance(typeof (ClassWithInstanceAttributes));


            instance.Name.ShouldEqual("Steve Bono"); // hey, he had a couple good years for awhile
            instance.Interceptors.Single()
                .ShouldBeOfType<ActivatorInterceptor<ClassWithInstanceAttributes>>();
        }
    }

    [InstanceName("Steve Bono")]
    [TurnOn]
    public class ClassWithInstanceAttributes
    {
        public void TurnOn()
        {
            
        }
    }

    public class TurnOnAttribute : InstanceAttribute
    {
        public override void Alter(IConfiguredInstance instance)
        {
            instance.AddInterceptor(new ActivatorInterceptor<ClassWithInstanceAttributes>(x => x.TurnOn()));
        }
    }

    public class InstanceNameAttribute : InstanceAttribute
    {
        private readonly string _name;

        public InstanceNameAttribute(string name)
        {
            _name = name;
        }

        public override void Alter(IConfiguredInstance instance)
        {
            instance.Name = _name;
        }
    }

    public class ClassWithArrayOfWidgets
    {
        private readonly IWidget[] _widgets;

        public ClassWithArrayOfWidgets(IWidget[] widgets)
        {
            _widgets = widgets;
        }

        public IWidget[] Widgets
        {
            get { return _widgets; }
        }
    }

    public class ClassWithNonPrimitive
    {
        public ClassWithNonPrimitive(IWidget widget)
        {
        }
    }

    public class ClassWithGuid
    {
        private readonly Guid _guid;

        public ClassWithGuid(Guid guid)
        {
            _guid = guid;
        }
    }

    public class ClassWithInt
    {
        private readonly int _age;

        public ClassWithInt(int age)
        {
            _age = age;
        }
    }

    public class ClassWithNullableInt
    {
        private readonly int? _age;

        public ClassWithNullableInt(int? age)
        {
            _age = age;
        }
    }

    public class ClassWithDate
    {
        private readonly DateTime _date;

        public ClassWithDate(DateTime date)
        {
            _date = date;
        }
    }

    public class ClassWithNullableDate
    {
        private readonly DateTime? _date;

        public ClassWithNullableDate(DateTime? date)
        {
            _date = date;
        }
    }
}