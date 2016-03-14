using Shouldly;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug438_Default_Instance_in_MissingNameInstance
    {
        [Fact]
        public void is_not_broken()
        {
            var container = new Container(_ =>
            {
                _.For<ISomeInterface>().Use(() => GetConcreteInstance());
                _.For<ISomeInterface>().MissingNamedInstanceIs.TheDefault();

                _.For<ISomeOtherThing>().Add<OtherConcreteClass>()
                    .Ctor<ISomeInterface>().IsNamedInstance("Special");
            });

            container.GetInstance<ISomeOtherThing>().As<OtherConcreteClass>()
                .Some.ShouldBeOfType<SomeClass>();
        }

        public class SomeClass : ISomeInterface { }

        public ISomeInterface GetConcreteInstance()
        {
            return new SomeClass();
        }

        public interface ISomeOtherThing
        {
        }

        public class OtherConcreteClass : ISomeOtherThing
        {
            public ISomeInterface Some { get; set; }

            public OtherConcreteClass(ISomeInterface some)
            {
                Some = some;
            }
        }

        public interface ISomeInterface { }
    }
}