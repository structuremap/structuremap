using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class do_not_allow_policy_changes_in_nested_container_issue_284
    {
        [Fact]
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