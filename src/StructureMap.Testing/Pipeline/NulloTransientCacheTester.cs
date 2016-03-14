using Moq;
using StructureMap.Pipeline;
using System;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class NulloTransientCacheTester
    {
        [Fact]
        public void get_must_build_the_object_in_the_current_session()
        {
            var session = new Mock<IBuildSession>();
            var instance = new ConfiguredInstance(typeof(Foo));

            var foo = new Foo(Guid.Empty);

            session.Setup(x => x.BuildNewInSession(typeof(IFoo), instance))
                .Returns(foo);

            new NulloTransientCache().Get(typeof(IFoo), instance, session.Object)
                .ShouldBeTheSameAs(foo);
        }
    }
}