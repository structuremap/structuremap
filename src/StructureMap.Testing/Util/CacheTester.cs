using NUnit.Framework;
using StructureMap.Testing.Widget;
using StructureMap.Util;

namespace StructureMap.Testing.Util
{
    [TestFixture]
    public class CacheTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void cloning_test()
        {
            var cache = new Cache<string, IWidget>(x => new ColorWidget(x));

            IWidget red = cache["red"];
            IWidget blue = cache["blue"];
            IWidget green = cache["green"];

            Cache<string, IWidget> clone = cache.Clone();

            clone["red"].ShouldBeTheSameAs(red);
            clone["blue"].ShouldBeTheSameAs(blue);
            clone["green"].ShouldBeTheSameAs(green);

            clone["purple"].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("purple");

            clone["purple"].ShouldNotBeTheSameAs(cache["purple"]);
        }
    }
}