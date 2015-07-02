using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class User_says_SetAllProperties_does_not_work
    {
        [Test]
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