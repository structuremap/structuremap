using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Building;
using StructureMap.Construction;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class BuildUpPlanTester
    {
        [Test]
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

            plan.BuildUp(session, target);

            target.Color.ShouldEqual("Red");
            target.Direction.ShouldEqual("Green");
            target.Gateway.ShouldBeTheSameAs(gateway);
        }
    }

    [TestFixture]
    public class BuildUpTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            TheDefaultGateway = new DefaultGateway();



            var args = new ExplicitArguments();
            args.Set<IGateway>(TheDefaultGateway);
            _session = BuildSession.Empty(args);

            theDependencies = new DependencyCollection {{"Age", 34}};



            _target = null;
        }

        #endregion

        private ClassWithMixOfSetters _target;
        private BuildSession _session;
        private DefaultGateway TheDefaultGateway;
        private DependencyCollection theDependencies;

        private ClassWithMixOfSetters TheTarget
        {
            get
            {
                if (_target == null)
                {
                    _target = new ClassWithMixOfSetters();

                    var thePlan = ConcreteType.BuildUpPlan(typeof (ClassWithMixOfSetters), theDependencies,
                        new Policies());

                    thePlan.BuildUp(_session, _target);
                }

                return _target;
            }
        }

        [Test]
        public void do_not_set_optional_properties_and_the_value_of_those_properties_is_the_default()
        {
            TheTarget.FirstName.ShouldBeNull();
            TheTarget.LastName.ShouldBeNull();
        }

        [Test]
        public void set_a_mandatory_primitive_property()
        {
            TheTarget.Age.ShouldEqual(34);
        }

        [Test]
        public void set_optional_properties_and_the_values_should_be_set()
        {
            theDependencies.Add("FirstName", "Jeremy");
            theDependencies.Add("LastName", "Miller");

            TheTarget.FirstName.ShouldEqual("Jeremy");
            TheTarget.LastName.ShouldEqual("Miller");
        }

        [Test]
        public void set_optional_property_for_a_child_object()
        {
            var theService = new ColorService("red");

            theDependencies.Add<IService>(theService);
            TheTarget.Service.ShouldBeTheSameAs(theService);
        }

        [Test]
        public void sets_a_mandatory_child_property()
        {
            TheTarget.Gateway.ShouldBeTheSameAs(TheDefaultGateway);
        }

        [Test]
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