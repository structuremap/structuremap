using NUnit.Framework;
using StructureMap.Testing.Widget;
using StructureMap.Util;

namespace StructureMap.Testing.Util
{
    [TestFixture]
    public class CacheTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void cloning_test()
        {
            var cache = new Cache<string, IWidget>(x => new ColorWidget(x));

            var red = cache["red"];
            var blue = cache["blue"];
            var green = cache["green"];

            var clone = cache.Clone();

            clone["red"].ShouldBeTheSameAs(red);
            clone["blue"].ShouldBeTheSameAs(blue);
            clone["green"].ShouldBeTheSameAs(green);

            clone["purple"].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("purple");

            clone["purple"].ShouldNotBeTheSameAs(cache["purple"]);
        }
    }
}