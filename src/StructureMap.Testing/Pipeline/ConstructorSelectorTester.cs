using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConstructorSelectorTester
    {
        [Test]
        public void get_the_first_constructor_marked_with_the_attribute_if_it_exists()
        {
            var selector = new ConstructorSelector();

            var constructor = selector.Select(typeof (ComplexRule));

            constructor.GetParameters().Length
                .ShouldEqual(7);
        }

        [Test]
        public void should_get_the_greediest_constructor_if_there_is_more_than_one()
        {
            var selector = new ConstructorSelector();
            var constructor = selector.Select(typeof (GreaterThanRule));

            constructor.GetParameters().Select(x => x.ParameterType)
                .ShouldHaveTheSameElementsAs(typeof (string), typeof (int));
        }

        [Test]
        public void custom_selectors_have_precedence()
        {
            var selector = new ConstructorSelector();
            selector.Add(new PickTheFirstOne());

            selector.Select(typeof(ClassWithMultipleConstructors))
                .GetParameters().Select(x => x.ParameterType)
                .ShouldHaveTheSameElementsAs(typeof(IGateway));
        }



        [Test]
        public void integration_test_with_custom_rule_in_container()
        {
            var container = new Container(x => {
                x.Policies.ConstructorSelector<PickTheFirstOne>();

                x.For<IGateway>().Use<StubbedGateway>();
                x.For<IService>().Use<WhateverService>();
                x.For<IWidget>().Use<AWidget>();
            });

            container.GetInstance<ClassWithMultipleConstructors>()
                .CtorUsed.ShouldEqual("One Arg");
        }


        public class PickTheFirstOne : IConstructorSelector
        {
            public ConstructorInfo Find(Type pluggedType)
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