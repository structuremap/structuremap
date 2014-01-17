using NUnit.Framework;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class setting_lifecycle_scope
    {
        [Test]
        public void can_specify_at_family_level()
        {
            var container = new Container(x => {
                x.For<Rule>().Singleton().Use<ARule>();
            });

            container.GetInstance<Rule>()
                .ShouldBeTheSameAs(container.GetInstance<Rule>())
                .ShouldBeTheSameAs(container.GetInstance<Rule>())
                .ShouldBeTheSameAs(container.GetInstance<Rule>());
        }

        [Test]
        public void can_override_lifecycle_at_instance()
        {
            Assert.Fail("Come back here, not done yet");

            var container = new Container(x => {
                x.For<Rule>().Singleton();

                x.For<Rule>().Add<ARule>().Named("A");
                x.For<Rule>().Add<ARule>().Named("B");
            });
        }
    }
}