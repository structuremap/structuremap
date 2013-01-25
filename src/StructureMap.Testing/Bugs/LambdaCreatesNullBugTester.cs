using System.Security.Principal;
using NUnit.Framework;

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
                x.For<IPrincipal>().Use(() => null);

                x.For<TestClass>().TheDefaultIsConcreteType<TestClass>();
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