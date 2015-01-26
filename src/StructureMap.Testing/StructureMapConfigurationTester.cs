using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing
{
    [TestFixture]
    public class StructureMapConfigurationTester
    {
        public class WebRegistry : Registry
        {
        }

        public class CoreRegistry : Registry
        {
        }

        [Test(
            Description =
                "Guid test based on problems encountered by Paul Segaro. See http://groups.google.com/group/structuremap-users/browse_thread/thread/34ddaf549ebb14f7?hl=en"
            )]
        public void TheDefaultInstanceIsALambdaForGuidNewGuid()
        {
            var container = new Container(x => x.For<Guid>().Use(() => Guid.NewGuid()));
            container.GetInstance<Guid>().ShouldNotEqual(Guid.Empty);
        }

        [Test(
            Description =
                "Guid test based on problems encountered by Paul Segaro. See http://groups.google.com/group/structuremap-users/browse_thread/thread/34ddaf549ebb14f7?hl=en"
            )]
        public void TheDefaultInstance_has_a_dependency_upon_a_Guid_NewGuid_lambda_generated_instance()
        {
            var container = new Container(x => {
                x.For<Guid>().Use(() => Guid.NewGuid());
                x.For<IFoo>().Use<Foo>();
            });

            container.GetInstance<IFoo>().SomeGuid.ShouldNotEqual(Guid.Empty);


        }
    }

    public interface IFoo
    {
        Guid SomeGuid { get; set; }
    }

    public class Foo : IFoo
    {
        public Foo(Guid someGuid)
        {
            SomeGuid = someGuid;
        }

        #region IFoo Members

        public Guid SomeGuid { get; set; }

        #endregion
    }

    public interface ISomething
    {
    }

    public class Something : ISomething
    {
        public Something()
        {
            throw new ApplicationException("You can't make me!");
        }
    }
}