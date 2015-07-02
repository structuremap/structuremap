using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class SmartInstanceTester
    {
        private SmartInstance<T, T> For<T>()
        {
            var instance = new SmartInstance<T, T>();

            return instance;
        }

        public T build<T>(Action<SmartInstance<T, T>> action)
        {
            var instance = For<T>();
            action(instance);

            var container = new Container(r => r.For<T>().UseInstance(instance));
            return container.GetInstance<T>();
        }

        [Test]
        public void specify_a_constructor_dependency()
        {
            var widget = new ColorWidget("Red");
            build<ClassWithWidget>(instance => instance.Ctor<IWidget>("widget").IsSpecial(x => x.Object(widget))).
                Widget.
                ShouldBeTheSameAs(widget);
        }

        [Test]
        public void specify_a_constructor_dependency_by_type()
        {
            var widget = new ColorWidget("Red");
            build<ClassWithWidget>(i => i.Ctor<IWidget>().IsSpecial(x => x.Object(widget))).Widget.ShouldBeTheSameAs(
                widget);
        }

        [Test]
        public void specify_a_non_simple_property_with_equal_to()
        {
            var widget = new ColorWidget("Red");
            var container = new Container(x => x.For<ClassWithWidgetProperty>()
                .Use<ClassWithWidgetProperty>()
                .Setter(o => o.Widget).Is(widget));

            widget.ShouldBeTheSameAs(container.GetInstance<ClassWithWidgetProperty>().Widget);
        }

        [Test]
        public void specify_a_property_dependency()
        {
            var widget = new ColorWidget("Red");
            build<ClassWithWidgetProperty>(i => i.Setter(x => x.Widget).IsSpecial(x => x.Object(widget))).Widget.
                ShouldBeTheSameAs(widget);

            var container = new Container(x =>
            {
                x.ForConcreteType<ClassWithWidgetProperty>().Configure
                    .Setter(o => o.Widget).IsSpecial(o => o.Object(new ColorWidget("Red")));
            });
        }

        [Test]
        public void specify_a_simple_property()
        {
            build<SimplePropertyTarget>(instance => instance.SetProperty(x => x.Name = "Jeremy")).Name.ShouldBe(
                "Jeremy");
            build<SimplePropertyTarget>(i => i.SetProperty(x => x.Age = 16)).Age.ShouldBe(16);

            var container = new Container(x =>
            {
                x.ForConcreteType<SimplePropertyTarget>().Configure
                    .SetProperty(target =>
                    {
                        target.Name = "Max";
                        target.Age = 4;
                    });
            });
        }

        [Test]
        public void specify_a_simple_property_name_with_equal_to()
        {
            build<SimplePropertyTarget>(i => i.Setter(x => x.Name).Is("Scott")).Name.ShouldBe("Scott");
        }

        [Test]
        public void specify_a_simple_property_with_equal_to()
        {
            build<SimplePropertyTarget>(i => i.Setter(x => x.Name).Is("Bret")).Name.ShouldBe("Bret");
        }


        [Test]
        public void specify_an_array_as_a_constructor()
        {
            IWidget widget1 = new AWidget();
            IWidget widget2 = new AWidget();
            IWidget widget3 = new AWidget();

            build<ClassWithWidgetArrayCtor>(i => i.EnumerableOf<IWidget>().Contains(x =>
            {
                x.Object(widget1);
                x.Object(widget2);
                x.Object(widget3);
            })).Widgets.ShouldBe(new[] {widget1, widget2, widget3});
        }


        [Test]
        public void specify_an_array_as_a_property()
        {
            IWidget widget1 = new AWidget();
            IWidget widget2 = new AWidget();
            IWidget widget3 = new AWidget();

            build<ClassWithWidgetArraySetter>(i => i.EnumerableOf<IWidget>().Contains(x =>
            {
                x.Object(widget1);
                x.Object(widget2);
                x.Object(widget3);
            })).Widgets.ShouldBe(new[] {widget1, widget2, widget3});
        }

        [Test]
        public void specify_ctorarg_with_non_simple_argument()
        {
            var widget = new ColorWidget("Red");
            var container = new Container(x => x.For<ClassWithWidget>()
                .Use<ClassWithWidget>()
                .Ctor<IWidget>().Is(widget));

            widget.ShouldBeTheSameAs(container.GetInstance<ClassWithWidget>().Widget);
        }

        [Test]
        public void successfully_specify_the_constructor_argument_of_a_string()
        {
            build<ColorRule>(i => i.Ctor<string>("color").Is("Red")).Color.ShouldBe("Red");
        }

        [Test]
        public void specify_a_constructor_dependency_by_name()
        {
            var container = new Container(r =>
            {
                r.For<ClassA>().Use<ClassA>().Ctor<ClassB>().Named("classB");
                r.For<ClassB>().Use<ClassB>().Named("classB").Ctor<string>("b").Is("named");
                r.For<ClassB>().Use<ClassB>().Ctor<string>("b").Is("default");
            });

            container.GetInstance<ClassA>()
                .B.B.ShouldBe("named");
        }

        [Test]
        public void smart_instance_can_specify_the_constructor()
        {
            new SmartInstance<ClassWithMultipleConstructors>(() => new ClassWithMultipleConstructors(null))
                .As<IConfiguredInstance>().Constructor.GetParameters().Select(x => x.ParameterType)
                .ShouldHaveTheSameElementsAs(typeof (IGateway));

            new SmartInstance<ClassWithMultipleConstructors>(() => new ClassWithMultipleConstructors(null, null))
                .As<IConfiguredInstance>().Constructor.GetParameters().Select(x => x.ParameterType)
                .ShouldHaveTheSameElementsAs(typeof (IGateway), typeof (IService));
        }

        [Test]
        public void integrated_building_with_distinct_ctor_selection()
        {
            var container = new Container(x =>
            {
                x.For<ClassWithMultipleConstructors>().AddInstances(o =>
                {
                    o.Type<ClassWithMultipleConstructors>()
                        .SelectConstructor(() => new ClassWithMultipleConstructors(null))
                        .Named("One");
                    o.Type<ClassWithMultipleConstructors>()
                        .SelectConstructor(() => new ClassWithMultipleConstructors(null, null))
                        .Named("Two");
                    o.Type<ClassWithMultipleConstructors>().Named("Default");
                });

                x.For<IGateway>().Use<StubbedGateway>();
                x.For<IService>().Use<WhateverService>();
                x.For<IWidget>().Use<AWidget>();
            });

            container.GetInstance<ClassWithMultipleConstructors>("One")
                .CtorUsed.ShouldBe("One Arg");

            container.GetInstance<ClassWithMultipleConstructors>("Two")
                .CtorUsed.ShouldBe("Two Args");

            container.GetInstance<ClassWithMultipleConstructors>("Default")
                .CtorUsed.ShouldBe("Three Args");
        }

        private class ClassA
        {
            public ClassB B { get; private set; }

            public ClassA(ClassB b)
            {
                B = b;
            }
        }

        private class ClassB
        {
            public string B { get; private set; }

            public ClassB(string b)
            {
                B = b;
            }
        }
    }

    public class ClassWithWidgetArrayCtor
    {
        private readonly IWidget[] _widgets;

        public ClassWithWidgetArrayCtor(IWidget[] widgets)
        {
            _widgets = widgets;
        }

        public IWidget[] Widgets
        {
            get { return _widgets; }
        }
    }

    public class ClassWithDoubleProperty
    {
        public double Double { get; set; }
    }

    public class ClassWithWidgetArraySetter
    {
        public IWidget[] Widgets { get; set; }
    }

    public class SimplePropertyTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class ClassWithWidget
    {
        private readonly IWidget _widget;

        public ClassWithWidget(IWidget widget)
        {
            _widget = widget;
        }

        public IWidget Widget
        {
            get { return _widget; }
        }
    }

    public class ClassWithWidgetProperty
    {
        public IWidget Widget { get; set; }
    }

    public class ClassWithMultipleConstructors
    {
        public string CtorUsed;

        public ClassWithMultipleConstructors(IGateway gateway, IService service, IWidget widget)
        {
            CtorUsed = "Three Args";
        }

        public ClassWithMultipleConstructors(IGateway gateway, IService service)
        {
            CtorUsed = "Two Args";
        }

        public ClassWithMultipleConstructors(IGateway gateway)
        {
            CtorUsed = "One Arg";
        }
    }
}