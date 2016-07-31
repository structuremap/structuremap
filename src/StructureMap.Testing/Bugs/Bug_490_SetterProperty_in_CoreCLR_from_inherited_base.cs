using StructureMap.Attributes;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_490_SetterProperty_in_CoreCLR_from_inherited_base
    {
        private Container container;

        public Bug_490_SetterProperty_in_CoreCLR_from_inherited_base()
        {
            container = new Container(_ =>
            {
                _.For<IMyService>().Use<MyService>();
                _.For<MyClass>().Use<MyClass>();
            });
        }

        public interface IMyService { }

        public class MyService : IMyService { }

        public class BaseClass
        {
            [SetterProperty]
            public IMyService MyServiceInBaseClass { get; set; }
        }

        public class MyClass : BaseClass
        {
            [SetterProperty]
            public IMyService MyServiceInMyClass { get; set; }
        }

        [Fact]
        public void property_should_be_injected_in_base_class()
        {
            var myClass = container.GetInstance<MyClass>();

            myClass.MyServiceInBaseClass.ShouldNotBeNull();
            myClass.MyServiceInMyClass.ShouldNotBeNull();
        }
    }
}