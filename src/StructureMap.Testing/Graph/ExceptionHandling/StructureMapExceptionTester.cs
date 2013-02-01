using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph.ExceptionHandling
{
    [TestFixture]
    public class StructureMapExceptionTester
    {
        private void assertErrorIsLogged(int errorCode, string xml)
        {
            PluginGraph graph = DataMother.BuildPluginGraphFromXml(xml);
            graph.Log.AssertHasError(errorCode);
        }


        private void assertErrorIsThrown(int errorCode, string xml, Action<Container> action)
        {
            var document = new XmlDocument();
            document.LoadXml(xml.Replace("\"", "'"));

            var parser = new ConfigurationParser(document.DocumentElement);
            var builder = new PluginGraphBuilder(parser);
            var manager = new Container(builder.Build());

            try
            {
                action(manager);
                Assert.Fail("Should have thrown exception");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(errorCode, ex.ErrorCode, "Expected error code");
            }
        }

        [Test]
        public void CanSerialize()
        {
            var ex = new ApplicationException("Oh no!");
            var smapEx = new StructureMapException(200, ex, "a", "b");

            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, smapEx);

            stream.Position = 0;

            var smapEx2 = (StructureMapException) formatter.Deserialize(stream);
            Assert.AreNotSame(smapEx, smapEx2);

            Assert.AreEqual(smapEx.Message, smapEx2.Message);
        }

        [Test]
        public void CouldNotFindInstanceKey()
        {
            assertErrorIsThrown(200,
                                @"
		            <StructureMap>
			            <Assembly Name='StructureMap.Testing.Widget'/>
            					
			            <PluginFamily Type='StructureMap.Testing.Widget.IWidget' Assembly='StructureMap.Testing.Widget' DefaultKey=''>			</PluginFamily>			
		            </StructureMap>
                ",
                                manager => manager.GetInstance<IWidget>("NotAnActualInstanceName")
                );
        }


        [Test]
        public void ExceptionMessage()
        {
            var exception = new StructureMapException(100, "StructureMap.config");
            string expected =
                "StructureMap Exception Code:  100\nExpected file \"StructureMap.config\" cannot be opened at StructureMap.config";

            string actual = exception.Message;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Throw_202_when_DefaultKeyDoesNotExist()
        {
            assertErrorIsThrown(202,
                                @"
		            <StructureMap>
			            <Assembly Name='StructureMap.Testing.Widget'/>
            					
			            <PluginFamily Type='StructureMap.Testing.Widget.IWidget' Assembly='StructureMap.Testing.Widget' DefaultKey=''></PluginFamily>			
		            </StructureMap>
                ",
                                manager => manager.GetInstance<IWidget>()
                );
        }
    }
}