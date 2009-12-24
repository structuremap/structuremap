using System;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConstructorInstanceTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void set_and_get_a_string()
        {
            var instance = ConstructorInstance.For<ColorWidget>();
            instance.SetValue("color", "Red");


            instance.Get("color", typeof (string), new StubBuildSession()).ShouldEqual("Red");

        }

        [Test]
        public void set_and_get_an_integer()
        {
            var instance = ConstructorInstance.For<ClassWithInt>();
            instance.SetValue("age", "45");

            instance.Get("age", typeof(int), new StubBuildSession()).ShouldEqual(45);
        }

        [Test]
        public void set_and_get_a_nullable_integer()
        {
            var instance = ConstructorInstance.For<ClassWithNullableInt>();
            instance.SetValue("age", "45");

            instance.Get("age", typeof(int?), new StubBuildSession()).ShouldEqual(45);

            instance.SetValue("age", null);
            instance.Get("age", typeof(int?), new StubBuildSession()).ShouldBeNull();
        }

        [Test]
        public void get_and_set_for_a_date_time()
        {
            var instance = ConstructorInstance.For<ClassWithDate>();
            instance.SetValue("date", "12/23/2009");

            instance.Get("date", typeof(DateTime), new StubBuildSession()).ShouldEqual(new DateTime(2009, 12, 23));
        }

        [Test]
        public void get_and_set_for_a_nullable_date_time()
        {
            var instance = ConstructorInstance.For<ClassWithNullableDate>();
            instance.SetValue("date", "12/23/2009");

            instance.Get("date", typeof(DateTime?), new StubBuildSession()).ShouldEqual(new DateTime(2009, 12, 23));

            instance.SetValue("date", null);

            instance.Get("date", typeof(DateTime?), new StubBuildSession()).ShouldBeNull();
        }

        [Test]
        public void get_and_set_for_a_guid()
        {
            var instance = ConstructorInstance.For<ClassWithGuid>();
            var guid = Guid.NewGuid();

            instance.SetValue("guid", guid.ToString());

            instance.Get("guid", typeof (Guid), new StubBuildSession()).ShouldEqual(guid);
        }

        [Test]
        public void get_and_set_a_non_primitive_type_that_is_not_enumerable()
        {
            var instance = ConstructorInstance.For<ClassWithNonPrimitive>();

            var widget = new AWidget();

            instance.SetValue("widget", widget);

            instance.Get("widget", typeof (IWidget), new StubBuildSession()).ShouldEqual(widget);
        }

        [Test]
        public void set_and_get_a_collection()
        {
            var children = new Instance[]
            {
                new SmartInstance<ColorWidget>().WithCtorArg("color").EqualTo("red"),
                new SmartInstance<ColorWidget>().WithCtorArg("color").EqualTo("green"),
                new ObjectInstance(new AWidget())
            };

            var instance = ConstructorInstance.For<ClassWithArrayOfWidgets>();
            instance.SetCollection("widgets", children);


            var widgets = instance.Get("widgets", typeof (IWidget), new StubBuildSession()).ShouldBeOfType<IWidget[]>();



            widgets.Length.ShouldEqual(3);

            widgets[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
            widgets[2].ShouldBeOfType<AWidget>();
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