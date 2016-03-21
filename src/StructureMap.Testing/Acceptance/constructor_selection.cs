using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using System;
using System.Reflection;
using Xunit;

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
            Assert.True(false, "I should not have been called");
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
        public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
        {
            // if this rule does not apply to the pluggedType,
            // just return null to denote "not applicable"
            if (!pluggedType.CanBeCastTo<BaseThing>()) return null;

            return pluggedType.GetConstructor(new[] { typeof(IWidget) });
        }
    }

    // ENDSAMPLE

    public class constructor_selection
    {
        // SAMPLE: using-custom-ctor-rule
        [Fact]
        public void use_a_custom_constructor_selector()
        {
            var container = new Container(_ =>
            {
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
            [DefaultConstructor]
            public AttributedThing(IWidget widget)
            {
                CorrectCtorWasUsed = true;
            }

            public bool CorrectCtorWasUsed { get; set; }

            public AttributedThing(IWidget widget, IService service)
            {
                Assert.True(false, "I should not have been called");
            }
        }

        [Fact]
        public void select_constructor_by_attribute()
        {
            var container = new Container(_ => { _.For<IWidget>().Use<AWidget>(); });

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
                Assert.True(false, "I should not have been called");
            }
        }

        [Fact]
        public void override_the_constructor_selection()
        {
            var container = new Container(_ =>
            {
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

        // SAMPLE: skip-ctor-with-missing-simples
        public class DbContext
        {
            public string ConnectionString { get; set; }

            public DbContext(string connectionString)
            {
                ConnectionString = connectionString;
            }

            public DbContext() : this("default value")
            {
            }
        }

        [Fact]
        public void should_bypass_ctor_with_unresolvable_simple_args()
        {
            var container = new Container();
            container.GetInstance<DbContext>()
                .ConnectionString.ShouldBe("default value");
        }

        [Fact]
        public void should_use_greediest_ctor_that_has_all_of_simple_dependencies()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<DbContext>().Configure
                    .Ctor<string>("connectionString").Is("not the default");
            });

            container.GetInstance<DbContext>()
                .ConnectionString.ShouldBe("not the default");
        }

        // ENDSAMPLE
    }
}