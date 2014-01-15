using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class SerializedInstanceTester
    {
        [Test]
        public void Build_object_from_serialization()
        {
            var colors = new Dictionary<string, string>();
            colors.Add("main", "red");
            colors.Add("border", "blue");


            var instance = new SerializedInstance(colors);

            Assert.Fail("NWO");
//            var colors2 = (Dictionary<string, string>) instance.Build(typeof (IDictionary<string, string>),
//                new StubBuildSession());
//
//            colors.ShouldNotBeTheSameAs(colors2);
//            colors2["main"].ShouldEqual("red");
        }
    }
}