using StructureMap.Pipeline;
using System.Security.Principal;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class LambdaCreatesNullBugTester
    {
        [Fact]
        public void Returning_null_values_in_constructed_by_generates_a_duplicate_cache_entry()
        {
            var container = new Container(x =>
            {
                x.For<IPrincipal>().UseInstance(new NullInstance());

                x.For<TestClass>().Use<TestClass>();
            });

            container.GetInstance<TestClass>().ShouldNotBeNull();

            //this throws a duplicate cache entry exception
            container.AssertConfigurationIsValid();
        }
    }

    public class TestClass
    {
        private IPrincipal principal;

        public TestClass(IPrincipal principal)
        {
            this.principal = principal;
        }
    }
}