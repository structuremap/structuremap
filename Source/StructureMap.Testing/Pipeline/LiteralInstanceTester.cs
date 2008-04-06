using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LiteralInstanceTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Build_happy_path()
        {
            ATarget target = new ATarget();
            LiteralInstance<ITarget> instance = new LiteralInstance<ITarget>(target);
            Assert.AreSame(target, instance.Build<ITarget>(null));
        }

        public interface ITarget
        {
            
        }

        public class ATarget : ITarget
        {
            public override string ToString()
            {
                return "the description of ATarget";
            }
        }
    }
}
