using System.Security.Principal;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class LambdaCreatesNullBugTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void Returning_null_values_in_contructed_by_generates_a_duplicate_cache_entry()
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