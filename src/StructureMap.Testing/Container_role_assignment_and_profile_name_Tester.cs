using NUnit.Framework;

namespace StructureMap.Testing
{
    [TestFixture]
    public class Container_role_assignment_and_profile_name_Tester
    {
        [Test]
        public void root_is_root()
        {
            new Container().Role.ShouldBe(ContainerRole.Root);
        }

        [Test]
        public void profile_container_is_profile()
        {
            new Container().GetProfile("Blue")
                .Role.ShouldBe(ContainerRole.ProfileOrChild);
        }

        [Test]
        public void profile_name()
        {
            var container = new Container();
            container.ProfileName.ShouldBe("DEFAULT");

            container.GetProfile("Blue").ProfileName.ShouldBe("Blue");

            container.GetProfile("Blue").GetNestedContainer().ProfileName.ShouldBe("Blue - Nested");
        }

        [Test]
        public void nested_container_from_the_root_is_nested()
        {
            new Container().GetNestedContainer()
                .Role.ShouldBe(ContainerRole.Nested);
        }

        [Test]
        public void nested_container_from_profile_is_nested()
        {
            new Container().GetProfile("Blue")
                .GetNestedContainer().Role.ShouldBe(ContainerRole.Nested);
        }
    }
}