using NestedLibrary;
using System.Linq;
using Xunit;
using Shouldly;

namespace StructureMap.Testing.Bugs
{
    public class Bug_629_convention_for_netstandard_20
    {
        [Fact]
        public void DefaultConventionFromBaseDirectory()
        {
            var container = new Container(r =>
            {
                r.Scan(x =>
                {
                    x.WithDefaultConventions();
                    x.AssembliesFromApplicationBaseDirectory();
                });
            });
            container.GetInstance<IFubu>().ShouldBeOfType<Fubu>();
        }
    }

    public class Fubu : IFubu
    {
    }

    public interface IFubu
    {
    }
}
