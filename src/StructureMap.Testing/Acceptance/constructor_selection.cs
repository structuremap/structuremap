using System;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Testing.Acceptance
{



    // SAMPLE: custom-ctor-scenario
    public abstract class BaseThing
    {
        public BaseThing(IWidget widget)
        {
            CorrectCtorWasUsed = true;
        }

        public bool CorrectCtorWasUsed { get; set; }

        public BaseThing(IWidget widget, IService service)
        {
            Assert.Fail("I should not have been called");
        }
    }

    public class Thing1 : BaseThing
    {
        public Thing1(IWidget widget) : base(widget)
        {
        }

        public Thing1(IWidget widget, IService service) : base(widget, service)
        {
        }
    }

    public class Thing2 : BaseThing
    {
        public Thing2(IWidget widget) : base(widget)
        {
        }

        public Thing2(IWidget widget, IService service) : base(widget, service)
        {
        }
    }
    // ENDSAMPLE

    // SAMPLE: custom-ctor-rule
    public class ThingCtorRule : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType)
        {
            // if this rule does not apply to the pluggedType,
            // just return null to denote "not applicable"
            if (!pluggedType.CanBeCastTo<BaseThing>()) return null;

            return pluggedType.GetConstructor(new Type[] {typeof (IWidget)});
        }
    }
    // ENDSAMPLE

    [TestFixture]
    public class constructor_selection
    {
        // SAMPLE: using-custom-ctor-rule
        [Test]
        public void use_a_custom_constructor_selector()
        {
            var container = new Container(_ => {
                _.For<IWidget>().Use<AWidget>();

                _.Policies.ConstructorSelector<ThingCtorRule>();
            });

            container.GetInstance<Thing1>()
                .CorrectCtorWasUsed.ShouldBeTrue();

            container.GetInstance<Thing2>()
                    .CorrectCtorWasUsed.ShouldBeTrue();
        }
        // ENDSAMPLE


        // SAMPLE: using-default-ctor-attribute
        public class AttributedThing
        {
            // Normally the greediest ctor would be
            // selected, but using this attribute
            // will overrid that behavior
            [StructureMap.DefaultConstructor]
            public AttributedThing(IWidget widget)
            {
                CorrectCtorWasUsed = true;
            }

            public bool CorrectCtorWasUsed { get; set; }

            public AttributedThing(IWidget widget, IService service)
            {
                Assert.Fail("I should not have been called");
            }
        }

        [Test]
        public void use_a_custom_constructor_rule()
        {
            var container = new Container(_ => {
                _.For<IWidget>().Use<AWidget>();
            });

            container.GetInstance<AttributedThing>()
                .CorrectCtorWasUsed
                .ShouldBeTrue();

        }
        // ENDSAMPLE

        // SAMPLE: explicit-ctor-selection
        public class Thingie
        {
            public Thingie(IWidget widget)
            {
                CorrectCtorWasUsed = true;
            }

            public bool CorrectCtorWasUsed { get; set; }

            public Thingie(IWidget widget, IService service)
            {
                Assert.Fail("I should not have been called");
            }
        }

        [Test]
        public void override_the_constructor_selection()
        {
            var container = new Container(_ => {
                _.For<IWidget>().Use<AWidget>();

                _.ForConcreteType<Thingie>().Configure

                    // StructureMap parses the expression passed
                    // into the method below to determine the 
                    // constructor
                    .SelectConstructor(() => new Thingie(null));
            });

            container.GetInstance<Thingie>()
                .CorrectCtorWasUsed
                .ShouldBeTrue();
        }
        // ENDSAMPLE

    }
}