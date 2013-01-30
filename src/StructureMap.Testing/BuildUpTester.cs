using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Construction;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class BuildUpTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            PluginCache.ResetAll();
            builder = PluginCache.FindBuilder(typeof (ClassWithMixOfSetters));
            instance = new SmartInstance<ClassWithMixOfSetters>().Ctor<int>("Age").EqualTo(34);
            _session = new BuildSession();

            TheDefaultGateway = new DefaultGateway();
            _session.RegisterDefault(typeof (IGateway), TheDefaultGateway);

            _target = null;
        }

        #endregion

        private IInstanceBuilder builder;
        private SmartInstance<ClassWithMixOfSetters> instance;
        private ClassWithMixOfSetters _target;
        private BuildSession _session;
        private DefaultGateway TheDefaultGateway;

        private ClassWithMixOfSetters TheTarget
        {
            get
            {
                if (_target == null)
                {
                    _target = new ClassWithMixOfSetters();

                    var args = new Arguments(instance, _session);

                    builder.BuildUp(args, _target);
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
            instance
                .Setter(x => x.FirstName).EqualTo("Jeremy")
                .Setter(x => x.LastName).EqualTo("Miller");

            TheTarget.FirstName.ShouldEqual("Jeremy");
            TheTarget.LastName.ShouldEqual("Miller");
        }

        [Test]
        public void set_optional_property_for_a_child_object()
        {
            var theService = new ColorService("red");
            instance.Setter(x => x.Service).Is(theService);

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