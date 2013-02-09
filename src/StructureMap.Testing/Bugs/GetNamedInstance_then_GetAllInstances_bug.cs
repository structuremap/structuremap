using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class GetNamedInstance_then_GetAllInstances_bug
    {
        [Test]
        public void do_not_blow_up()
        {
            var container = new Container(x =>
            {
                
            });

            container.GetInstance<Something>("foo");
            container.GetAllInstances<Something>();
        }
    }

    public class Something{}
}