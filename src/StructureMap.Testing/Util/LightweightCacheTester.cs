using StructureMap.Util;
using Xunit;

namespace StructureMap.Testing.Util
{
    public class LightweightCacheTester
    {
        [Fact]
        public void can_create_it_without_the_clr_wigging_out()
        {
            var cache = new LightweightCache<string, string>();
        }
    }
}