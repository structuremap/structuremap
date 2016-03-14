using Shouldly;
using StructureMap.Testing.Widget2;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class EnumerationTester
    {
        [Fact]
        public void BuildClassWithEnumeration()
        {
            var manager = new Container();

            manager.Configure(r => r.For<Cow>().Use<Cow>()
                .Named("Angus")
                .Ctor<string>("Name").Is("Bessie")
                .Ctor<BreedEnum>("Breed").Is(BreedEnum.Angus)
                .Ctor<long>("Weight").Is(1200));

            var angus = manager.GetInstance<Cow>("Angus");

            angus.ShouldNotBeNull();
            angus.Name.ShouldBe("Bessie");
            angus.Breed.ShouldBe(BreedEnum.Angus);
            angus.Weight.ShouldBe(1200);
        }
    }
}