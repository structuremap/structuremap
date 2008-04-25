using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class BuildStrategiesTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void Singleton_build_policy()
        {
            SingletonPolicy policy = new SingletonPolicy();
            ConstructorInstance instance1 = new ConstructorInstance(delegate { return new ColorService("Red"); }).WithName("Red");
            ConstructorInstance instance2 = new ConstructorInstance(delegate { return new ColorService("Green"); }).WithName("Green");

            ColorService red1 = (ColorService) policy.Build(new StubInstanceCreator(), null, instance1);
            ColorService green1 = (ColorService)policy.Build(new StubInstanceCreator(), null, instance2);
            ColorService red2 = (ColorService)policy.Build(new StubInstanceCreator(), null, instance1);
            ColorService green2 = (ColorService)policy.Build(new StubInstanceCreator(), null, instance2);
            ColorService red3 = (ColorService)policy.Build(new StubInstanceCreator(), null, instance1);
            ColorService green3 = (ColorService)policy.Build(new StubInstanceCreator(), null, instance2);
        
            Assert.AreSame(red1, red2);
            Assert.AreSame(red1, red3);
            Assert.AreSame(green1, green2);
            Assert.AreSame(green1, green3);
        }

        [Test]
        public void CloneSingleton()
        {
            SingletonPolicy policy = new SingletonPolicy();

            SingletonPolicy clone = (SingletonPolicy) policy.Clone();
            Assert.AreNotSame(policy, clone);
            Assert.IsInstanceOfType(typeof(BuildPolicy), clone.InnerPolicy);
        }

        [Test]
        public void CloneHybrid()
        {
            HybridBuildPolicy policy = new HybridBuildPolicy();

            HybridBuildPolicy clone = (HybridBuildPolicy)policy.Clone();
            Assert.AreNotSame(policy, clone);
            Assert.IsInstanceOfType(typeof(BuildPolicy), clone.InnerPolicy);
        }



    }
}