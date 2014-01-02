using System;
using NUnit.Framework;
using StructureMap.Building;
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
            var children = new Instance[]
            {
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("red"),
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("green"),
                new ObjectInstance(new AWidget())
            };

            var instance = ConstructorInstance.For<ClassWithArrayOfWidgets>();
            instance.Dependencies.Add("widgets", children);

            var widgets = instance.Build(typeof (ClassWithArrayOfWidgets), new StubBuildSession())
                .As<ClassWithArrayOfWidgets>()
                .Widgets;

            widgets.Length.ShouldEqual(3);

            widgets[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
            widgets[2].ShouldBeOfType<AWidget>();
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