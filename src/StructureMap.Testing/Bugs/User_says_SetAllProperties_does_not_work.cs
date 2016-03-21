using Shouldly;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class User_says_SetAllProperties_does_not_work
    {
        [Fact]
        public void try_the_setter_policy()
        {
            var container = new Container(_ =>
            {
                _.Policies.FillAllPropertiesOfType<IApiCommunicator>()
                    .Use<ApiCommunicatorWindowsAuthentication>();
            });

            container.GetInstance<BaseController>()
                .ApiCommunicator.ShouldBeOfType<ApiCommunicatorWindowsAuthentication>();
        }
    }

    public interface IApiCommunicator
    {
    }

    public class ApiCommunicatorWindowsAuthentication : IApiCommunicator
    {
    }

    public class BaseController
    {
        public IApiCommunicator ApiCommunicator { get; set; }
    }
}