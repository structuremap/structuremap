using StructureMap.Graph;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_247_ConnectOpenTypesToImplementations_doubling_up_registrations
    {
        [Fact]
        public void Scanner_apply_should_only_register_two_instances()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.ConnectImplementationsToTypesClosing(typeof(ISomeServiceOf<>));
                });
            });

            container.GetAllInstances<ISomeServiceOf<string>>().OrderBy(x => x.GetType().Name).Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(SomeService1), typeof(SomeService2));
        }

        public interface ISomeServiceOf<T>
        {
        }

        public class SomeService1 : ISomeServiceOf<string>
        {
        }

        public class SomeService2 : ISomeServiceOf<string>
        {
        }
    }
}