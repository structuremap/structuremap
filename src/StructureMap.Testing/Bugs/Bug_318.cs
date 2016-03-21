using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_318
    {
        [Fact]
        public void recreate_the_problem()
        {
            var container = Container.For<TestStructureMapRegistry>();

            var service1 = container.GetInstance<MyService1>();
            service1.Dep1.ShouldNotBeTheSameAs(service1.Dep2);

            var service2 = container.GetInstance<MyService2>();
            service2.Dep1.ShouldNotBeTheSameAs(service2.Dep2);
        }

        public class TestStructureMapRegistry : Registry
        {
            public TestStructureMapRegistry()
            {
                For<MyService1>().Use<MyService1>()
                    .Ctor<MyDependency>("dep1").Is(new MyDependency())
                    .Ctor<MyDependency>("dep2").Is(new MyDependency());

                For<MyService2>().Use<MyService2>()
                    .Ctor<MyDependency>("dep1").Is(new MyDependency())
                    .Ctor<MyDependency>("dep2").Is(new MyDependency());
            }
        }

        public class MyDependency
        {
        }

        public class MyService1
        {
            // Just mark these two as private setters and the test above passes
            public MyDependency Dep1 { get; private set; }

            public MyDependency Dep2 { get; private set; }

            public MyService1(MyDependency dep1, MyDependency dep2)
            {
                this.Dep1 = dep1;
                this.Dep2 = dep2;
            }
        }

        public class MyService2
        {
            private readonly MyDependency dep1;
            private readonly MyDependency dep2;

            public MyDependency Dep1
            {
                get { return dep1; }
            }

            public MyDependency Dep2
            {
                get { return dep2; }
            }

            public MyService2(MyDependency dep1, MyDependency dep2)
            {
                this.dep1 = dep1;
                this.dep2 = dep2;
            }
        }
    }
}