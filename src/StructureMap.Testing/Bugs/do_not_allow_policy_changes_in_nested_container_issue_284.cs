using NUnit.Framework;
using StructureMap.Testing.Acceptance;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class do_not_allow_policy_changes_in_nested_container_issue_284
    {
        [Test]
        public void do_not_allow()
        {
            var container = new Container();

            Exception<StructureMapConfigurationException>.ShouldBeThrownBy(
                () =>
                {
                    container.GetNestedContainer().Configure(_ => { _.Policies.FillAllPropertiesOfType<IWidget>(); });
                });
        }
    }
}