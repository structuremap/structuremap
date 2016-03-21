using Shouldly;
using Xunit;

namespace StructureMap.Testing
{
    public class Container_role_assignment_and_profile_name_Tester
    {
        [Fact]
        public void root_is_root()
        {
            new Container().Role.ShouldBe(ContainerRole.Root);
        }

        [Fact]
        public void profile_container_is_profile()
        {
            new Container().GetProfile("Blue")
                .Role.ShouldBe(ContainerRole.ProfileOrChild);
        }

        [Fact]
        public void profile_name()
        {
            var container = new Container();
            container.ProfileName.ShouldBe("DEFAULT");

            container.GetProfile("Blue").ProfileName.ShouldBe("Blue");

            container.GetProfile("Blue").GetNestedContainer().ProfileName.ShouldBe("Blue - Nested");
        }

        [Fact]
        public void nested_container_from_the_root_is_nested()
        {
            new Container().GetNestedContainer()
                .Role.ShouldBe(ContainerRole.Nested);
        }

        [Fact]
        public void nested_container_from_profile_is_nested()
        {
            new Container().GetProfile("Blue")
                .GetNestedContainer().Role.ShouldBe(ContainerRole.Nested);
        }
    }
}