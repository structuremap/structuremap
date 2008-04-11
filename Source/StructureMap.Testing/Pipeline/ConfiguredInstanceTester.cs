using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConfiguredInstanceTester
    {
        private ConfiguredInstance instance;

        [SetUp]
        public void SetUp()
        {
            instance = new ConfiguredInstance();
        }

        [Test]
        public void GetProperty_happy_path()
        {
            instance.SetProperty("Color", "Red")
                    .SetProperty("Age", "34");

            IConfiguredInstance configuredInstance = instance;

            Assert.AreEqual("Red", configuredInstance.GetProperty("Color"));
            Assert.AreEqual("34", configuredInstance.GetProperty("Age"));

            instance.SetProperty("Color", "Blue");
            Assert.AreEqual("Blue", configuredInstance.GetProperty("Color"));
        }

        [Test]
        public void Property_cannot_be_found_so_throw_205()
        {
            try
            {
                IConfiguredInstance configuredInstance = instance;
                configuredInstance.GetProperty("anything");
                Assert.Fail("Did not throw exception");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(205, ex.ErrorCode);
            }
        }
    }
}
