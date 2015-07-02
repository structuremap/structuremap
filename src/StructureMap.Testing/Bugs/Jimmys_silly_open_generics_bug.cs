using System.Linq;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Jimmys_silly_open_generics_bug
    {
        [Test]
        public void fix_it()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();


                    scan.ConnectImplementationsToTypesClosing(typeof (IBird<>));
                });
            });

            container.GetAllInstances<IBird<Bird>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (BirdImpl), typeof (BirdBaseImpl));
        }

        [Test]
        public void GenericConnectionScanner_can_match_on_it()
        {
            typeof (BirdBaseImpl).CanBeCastTo<IBird<Bird>>()
                .ShouldBeTrue();

            var scanner = new GenericConnectionScanner(typeof (IBird<>));
            scanner.Process(typeof (BirdBaseImpl), new Registry());
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