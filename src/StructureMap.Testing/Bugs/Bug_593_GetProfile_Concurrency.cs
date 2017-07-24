using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_593_GetProfile_Concurrency
    {
        [Fact]
        public void GetProfileStressTest()
        {
            const int profileCount = 5;
            var profileNames = Enumerable.Range(0, profileCount).Select(_ => Guid.NewGuid().ToString()).ToList();
            var container = new Container(cfg =>
            {
                foreach (var profileName in profileNames)
                {
                    cfg.Profile(profileName, _ => { });
                }
            });

            Parallel.For(0, 10000, i =>
            {
                container.GetProfile(profileNames[i % profileCount]);
            });
        }
    }
}