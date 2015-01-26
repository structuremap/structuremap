using NUnit.Framework;
using StructureMap.Util;

namespace StructureMap.Testing.Util
{
    [TestFixture]
    public class LightweightCacheTester
    {
        [Test]
        public void can_create_it_without_the_clr_wigging_out()
        {
            var cache = new LightweightCache<string, string>();
        }
    }
}