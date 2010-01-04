using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConditionalInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            instance = new ConditionalInstance<Rule>(x =>
            {
                x.If(context => context.RequestedName == "red").ThenIt.IsThis(Red);
                x.If(context => context.RequestedName == "green").ThenIt.IsThis(Green);
                x.If(context => context.RequestedName == "blue").ThenIt.IsThis(Blue);
                x.TheDefault.IsThis(Default);
            });


            _session = new BuildSession();
        }

        #endregion

        private ConditionalInstance<Rule> instance;
        private readonly Rule Red = new ColorRule("red");
        private readonly Rule Green = new ColorRule("green");
        private readonly Rule Blue = new ColorRule("blue");
        private readonly Rule Default = new ColorRule("black");

        private BuildSession _session;

        private void TheRequestedNameIs(string name)
        {
            _session.RequestedName = name;
        }

        private void TheConstructedRuleShouldBe(Rule expected)
        {
            instance.Build(typeof (Rule), _session).ShouldBeTheSameAs(expected);
        }

        [Test]
        public void catch_the_first_conditional()
        {
            TheRequestedNameIs("red");
            TheConstructedRuleShouldBe(Red);
        }


        [Test]
        public void catch_the_second_conditional()
        {
            TheRequestedNameIs("green");
            TheConstructedRuleShouldBe(Green);
        }


        [Test]
        public void catch_the_third_conditional()
        {
            TheRequestedNameIs("blue");
            TheConstructedRuleShouldBe(Blue);
        }

        [Test]
        public void use_the_default_if_nothing_matches()
        {
            TheRequestedNameIs("nothing that matches");
            TheConstructedRuleShouldBe(Default);
        }
    }

    [TestFixture]
    public class ConditionalInstanceIntegratedTester
    {
        private readonly Rule RED = new ColorRule("red");
        private readonly Rule GREEN = new ColorRule("green");
        private Rule Blue = new ColorRule("blue");
        private readonly Rule DEFAULT = new ColorRule("black");

        [Test]
        public void use_a_conditional_instance_if_the_test_is_true()
        {
            var container = new Container(x =>
            {
                x.For<Rule>().TheDefault.IsThis(DEFAULT);
                x.InstanceOf<Rule>().Is.Conditional(o =>
                {
                    o.If(c => false).ThenIt.Is.OfConcreteType<ARule>();
                    o.If(c => true).ThenIt.IsThis(GREEN);
                    o.TheDefault.IsThis(RED);
                }).WithName("conditional");
            });

            container.GetInstance<Rule>("conditional").ShouldBeTheSameAs(GREEN);
        }

        [Test]
        public void use_the_explicit_default_for_the_conditional_when_it_is_specified()
        {
            var container = new Container(x =>
            {
                x.For<Rule>().TheDefault.IsThis(DEFAULT);
                x.InstanceOf<Rule>().Is.Conditional(o =>
                {
                    o.If(c => false).ThenIt.Is.OfConcreteType<ARule>();
                    o.TheDefault.IsThis(RED);
                }).WithName("conditional");
            });

            container.GetInstance<Rule>("conditional").ShouldBeTheSameAs(RED);
        }

        [Test]
        public void use_the_normal_default_if_it_is_not_specified_and_nothing_matches()
        {
            var container = new Container(x =>
            {
                x.For<Rule>().TheDefault.IsThis(DEFAULT);
                x.InstanceOf<Rule>().Is.Conditional(o => { o.If(c => false).ThenIt.Is.OfConcreteType<ARule>(); }).
                    WithName("conditional");
            });

            container.GetInstance<Rule>("conditional").ShouldBeTheSameAs(DEFAULT);
        }
    }
}