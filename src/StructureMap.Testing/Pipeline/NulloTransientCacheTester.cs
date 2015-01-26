using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class NulloTransientCacheTester
    {
        [Test]
        public void get_must_build_the_object_in_the_current_session()
        {
            var session = MockRepository.GenerateMock<IBuildSession>();
            var instance = new ConfiguredInstance(typeof (Foo));

            var foo = new Foo(Guid.Empty);

            session.Stub(x => x.BuildNewInSession(typeof (IFoo), instance))
                .Return(foo);

            new NulloTransientCache().Get(typeof (IFoo), instance, session)
                .ShouldBeTheSameAs(foo);
        }
    }
}