using StructureMap.Graph;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Jimmys_silly_open_generics_bug
    {
        [Fact]
        public void fix_it()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();

                    scan.ConnectImplementationsToTypesClosing(typeof(IBird<>));
                });
            });

            container.GetAllInstances<IBird<Bird>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(BirdImpl), typeof(BirdBaseImpl));
        }
    }

    public interface IBird<in T>
    {
    }

    public class BirdBase
    {
    }

    public class Bird : BirdBase
    {
    }

    public class BirdImpl : IBird<Bird>
    {
    }

    public class BirdBaseImpl : IBird<BirdBase>
    {
    }
}