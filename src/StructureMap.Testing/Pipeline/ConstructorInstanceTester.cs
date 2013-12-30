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
        public void get_and_set_a_non_primitive_type_that_is_not_enumerable()
        {
            ConstructorInstance instance = ConstructorInstance.For<ClassWithNonPrimitive>();

            var widget = new AWidget();

            instance.As<IConfiguredInstance>().SetValue("widget", widget);

            instance.Get("widget", typeof (IWidget), new StubBuildSession()).ShouldEqual(widget);
        }

        [Test]
        public void get_and_set_for_a_date_time()
        {
            ConstructorInstance instance = ConstructorInstance.For<ClassWithDate>();
            instance.As<IConfiguredInstance>().SetValue("date", "12/23/2009");

            instance.Get("date", typeof (DateTime), new StubBuildSession()).ShouldEqual(new DateTime(2009, 12, 23));
        }

        [Test]
        public void get_and_set_for_a_guid()
        {
            ConstructorInstance instance = ConstructorInstance.For<ClassWithGuid>();
            Guid guid = Guid.NewGuid();

            instance.As<IConfiguredInstance>().SetValue("guid", guid.ToString());

            instance.Get("guid", typeof (Guid), new StubBuildSession()).ShouldEqual(guid);
        }

        [Test]
        public void get_and_set_for_a_nullable_date_time()
        {
            ConstructorInstance instance = ConstructorInstance.For<ClassWithNullableDate>();
            instance.As<IConfiguredInstance>().SetValue("date", "12/23/2009");

            instance.Get("date", typeof (DateTime?), new StubBuildSession()).ShouldEqual(new DateTime(2009, 12, 23));

            instance.As<IConfiguredInstance>().SetValue("date", null);

            instance.Get("date", typeof (DateTime?), new StubBuildSession()).ShouldBeNull();
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

            ConstructorInstance instance = ConstructorInstance.For<ClassWithArrayOfWidgets>();
            instance.As<IConfiguredInstance>().SetCollection("widgets", children);


            var widgets = instance.Get("widgets", typeof (IWidget), new StubBuildSession()).ShouldBeOfType<IWidget[]>();


            widgets.Length.ShouldEqual(3);

            widgets[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
            widgets[2].ShouldBeOfType<AWidget>();
        }

        [Test]
        public void set_and_get_a_nullable_integer()
        {
            ConstructorInstance instance = ConstructorInstance.For<ClassWithNullableInt>();
            instance.As<IConfiguredInstance>().SetValue("age", "45");

            instance.Get("age", typeof (int?), new StubBuildSession()).ShouldEqual(45);

            instance.As<IConfiguredInstance>().SetValue("age", null);
            instance.Get("age", typeof (int?), new StubBuildSession()).ShouldBeNull();
        }

        [Test]
        public void set_and_get_a_string()
        {
            ConstructorInstance instance = ConstructorInstance.For<ColorWidget>();
            instance.As<IConfiguredInstance>().SetValue("color", "Red");


            instance.Get("color", typeof (string), new StubBuildSession()).ShouldEqual("Red");
        }

        [Test]
        public void set_and_get_an_integer()
        {
            ConstructorInstance instance = ConstructorInstance.For<ClassWithInt>();
            instance.As<IConfiguredInstance>().SetValue("age", "45");

            instance.Get("age", typeof (int), new StubBuildSession()).ShouldEqual(45);
        }
    }

    public class ClassWithArrayOfWidgets
    {
        public ClassWithArrayOfWidgets(IWidget[] widgets)
        {
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