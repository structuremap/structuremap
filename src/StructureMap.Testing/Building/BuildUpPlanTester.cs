using Shouldly;
using StructureMap.Attributes;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class BuildUpPlanTester
    {
        [Fact]
        public void can_build_setters_on_an_existing_object()
        {
            var target = new SetterTarget();

            var gateway = new StubbedGateway();
            var session = new FakeBuildSession();
            session.SetDefault<IGateway>(gateway);

            var plan = new BuildUpPlan<SetterTarget>();
            plan.Set(x => x.Color, "Red");
            plan.Set(x => x.Direction, "Green");
            plan.Set(x => x.Gateway, new DefaultDependencySource(typeof(IGateway)));

            plan.BuildUp(session, session, target);

            target.Color.ShouldBe("Red");
            target.Direction.ShouldBe("Green");
            target.Gateway.ShouldBeTheSameAs(gateway);
        }
    }

    public class BuildUpTester
    {
        public BuildUpTester()
        {
            theDefaultGateway = new DefaultGateway();

            var args = new ExplicitArguments();
            args.Set<IGateway>(theDefaultGateway);
            session = BuildSession.Empty(args);

            theDependencies = new DependencyCollection { { "Age", 34 } };

            target = null;
        }

        private ClassWithMixOfSetters target;
        private readonly BuildSession session;
        private readonly DefaultGateway theDefaultGateway;
        private DependencyCollection theDependencies;

        private ClassWithMixOfSetters TheTarget
        {
            get
            {
                if (target == null)
                {
                    target = new ClassWithMixOfSetters();

                    var thePlan = ConcreteType.BuildUpPlan(typeof(ClassWithMixOfSetters), theDependencies,
                        Policies.Default());

                    thePlan.BuildUp(session, session, target);
                }

                return target;
            }
        }

        [Fact]
        public void do_not_set_optional_properties_and_the_value_of_those_properties_is_the_default()
        {
            TheTarget.FirstName.ShouldBeNull();
            TheTarget.LastName.ShouldBeNull();
        }

        [Fact]
        public void set_a_mandatory_primitive_property()
        {
            TheTarget.Age.ShouldBe(34);
        }

        [Fact]
        public void set_optional_properties_and_the_values_should_be_set()
        {
            theDependencies.Add("FirstName", "Jeremy");
            theDependencies.Add("LastName", "Miller");

            TheTarget.FirstName.ShouldBe("Jeremy");
            TheTarget.LastName.ShouldBe("Miller");
        }

        [Fact]
        public void set_optional_property_for_a_child_object()
        {
            var theService = new ColorService("red");

            theDependencies.Add<IService>(theService);
            TheTarget.Service.ShouldBeTheSameAs(theService);
        }

        [Fact]
        public void sets_a_mandatory_child_property()
        {
            TheTarget.Gateway.ShouldBeTheSameAs(theDefaultGateway);
        }

        [Fact]
        public void the_optional_child_properties_are_not_set_if_the_instance_is_not_explicitly_specified()
        {
            TheTarget.Service.ShouldBeNull();
        }
    }

    public class ClassWithMixOfSetters
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [SetterProperty]
        public int Age { get; set; }

        [SetterProperty]
        public IGateway Gateway { get; set; }

        public IService Service { get; set; }
    }
}