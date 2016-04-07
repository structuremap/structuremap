using Shouldly;
using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class disposing_nested_container_created_from_profile
    {
        private class ProfileRegistry : Registry
        {
            public ProfileRegistry()
            {
                Profile("my-profile", p => p.For<IService>().Use<AService>());
            }
        }

        [Fact]
        public void nested_container_from_profile()
        {
            var parent = new Container(_ =>
            {
                _.AddRegistry<ProfileRegistry>();
            });

            // Create a child container and override the
            // IService registration
            var child = parent.GetProfile("my-profile");

            using (var nested = child.GetNestedContainer())
            {
                nested.GetInstance<IService>()
                    .ShouldBeOfType<AService>();
            }

            using (var nested = child.GetNestedContainer())
            {
                nested.GetInstance<IService>()
                    .ShouldBeOfType<AService>();
            }
        }
    }
}