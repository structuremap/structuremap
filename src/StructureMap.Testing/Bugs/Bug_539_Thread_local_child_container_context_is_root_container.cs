using Shouldly;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_539_Thread_local_child_container_context_is_root_container
    {
        [Fact]
        public void thread_local_container_context_should_match_container_instance()
        {
            var container = new Container();
            container.Configure(x =>
            {
                x.For<ISession>().LifecycleIs(Lifecycles.ThreadLocal).Use(c => new Session(c.GetInstance<IContainer>()));
            });

            {
                var child = container.GetNestedContainer();
                child.Configure(x =>
                {
                    x.For<ISession>().Use<Session>();
                });

                container.GetInstance<ISession>().Container.ShouldBe(container);
                child.GetInstance<ISession>().Container.ShouldNotBe(container);
                child.GetInstance<ISession>().Container.ShouldBe(child);
            }
        }

        public interface ISession
        {
            IContainer Container { get; }
        }

        public class Session : ISession
        {
            public IContainer Container { get; }

            public Session(IContainer container)
            {
                Container = container;
            }
        }
    }
}
