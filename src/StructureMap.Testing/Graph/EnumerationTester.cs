using NUnit.Framework;
using Shouldly;
using StructureMap.Testing.Widget2;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class EnumerationTester
    {
        [Test]
        public void BuildClassWithEnumeration()
        {
            var manager = new Container();

            manager.Configure(r => r.For<Cow>().Use<Cow>()
                .Named("Angus")
                .Ctor<string>("Name").Is("Bessie")
                .Ctor<BreedEnum>("Breed").Is(BreedEnum.Angus)
                .Ctor<long>("Weight").Is(1200));

            var angus = manager.GetInstance<Cow>("Angus");

            angus.IsNotNull();
            angus.Name.ShouldBe("Bessie");
            angus.Breed.ShouldBe(BreedEnum.Angus);
            angus.Weight.ShouldBe(1200);
        }
    }
}