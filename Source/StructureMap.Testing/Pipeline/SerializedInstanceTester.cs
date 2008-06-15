using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
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
            Dictionary<string, string> colors = new Dictionary<string, string>();
            colors.Add("main", "red");
            colors.Add("border", "blue");


            SerializedInstance instance = new SerializedInstance(colors);

            Dictionary<string, string> colors2 = (Dictionary<string, string>) instance.Build(typeof (IDictionary<string, string>),
                                                                             new StubBuildSession());

            colors.ShouldNotBeTheSameAs(colors2);
            colors2["main"].ShouldEqual("red");
        }
    }
}
