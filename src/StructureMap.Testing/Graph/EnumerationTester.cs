using NUnit.Framework;
using StructureMap.Graph;
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

            Assert.IsNotNull(angus);
            Assert.AreEqual("Bessie", angus.Name, "Name");
            Assert.AreEqual(BreedEnum.Angus, angus.Breed, "Breed");
            Assert.AreEqual(1200, angus.Weight, "Weight");
        }
    }
}