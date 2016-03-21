using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class ConstructorSelectorTester
    {
        [Fact]
        public void get_the_first_constructor_marked_with_the_attribute_if_it_exists()
        {
            var selector = new ConstructorSelector(PluginGraph.CreateRoot());

            var constructor = selector.Select(typeof(ComplexRule), new DependencyCollection());

            constructor.GetParameters().Length
                .ShouldBe(7);
        }

        // SAMPLE: select-the-greediest-ctor
        public class GreaterThanRule : Rule
        {
            public string Attribute { get; set; }
            public int Value { get; set; }

            public GreaterThanRule()
            {
            }

            public GreaterThanRule(string attribute, int value)
            {
                Attribute = attribute;
                Value = value;
            }

            public GreaterThanRule(IWidget widget, Rule rule)
            {
            }
        }

        [Fact]
        public void using_the_greediest_ctor()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<GreaterThanRule>().Configure
                    .Ctor<string>("attribute").Is("foo")
                    .Ctor<int>("value").Is(42);
            });

            var rule = container.GetInstance<GreaterThanRule>();
            rule.Attribute.ShouldBe("foo");
            rule.Value.ShouldBe(42);
        }

        // ENDSAMPLE

        [Fact]
        public void should_get_the_greediest_constructor_if_there_is_more_than_one()
        {
            var selector = new ConstructorSelector(PluginGraph.CreateRoot());
            var constructor = selector.Select(typeof(GreaterThanRule), new DependencyCollection());

            constructor.GetParameters().Select(x => x.ParameterType)
                .ShouldHaveTheSameElementsAs(typeof(IWidget), typeof(Rule));
        }

        [Fact]
        public void custom_selectors_have_precedence()
        {
            var selector = new ConstructorSelector(PluginGraph.CreateRoot());
            selector.Add(new PickTheFirstOne());

            selector.Select(typeof(ClassWithMultipleConstructors), new DependencyCollection())
                .GetParameters().Select(x => x.ParameterType)
                .ShouldHaveTheSameElementsAs(typeof(IGateway));
        }

        [Fact]
        public void integration_test_with_custom_rule_in_container()
        {
            var container = new Container(x =>
            {
                x.Policies.ConstructorSelector<PickTheFirstOne>();

                x.For<IGateway>().Use<StubbedGateway>();
                x.For<IService>().Use<WhateverService>();
                x.For<IWidget>().Use<AWidget>();
            });

            container.GetInstance<ClassWithMultipleConstructors>()
                .CtorUsed.ShouldBe("One Arg");
        }

        public class PickTheFirstOne : IConstructorSelector
        {
            public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
            {
                return pluggedType.GetConstructors().OrderBy(x => x.GetParameters().Count()).FirstOrDefault();
            }
        }

        public class ClassWithMultipleConstructors
        {
            public string CtorUsed;

            public ClassWithMultipleConstructors(IGateway gateway)
            {
                CtorUsed = "One Arg";
            }

            public ClassWithMultipleConstructors(IGateway gateway, IService service)
            {
                CtorUsed = "Two Args";
            }

            public ClassWithMultipleConstructors(IGateway gateway, IService service, IWidget widget)
            {
                CtorUsed = "Three Args";
            }
        }
    }
}