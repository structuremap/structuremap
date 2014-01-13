using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Configuration.Xml;
using StructureMap.Testing;
using StructureMap.Testing.Widget;

namespace StructureMap.Xml.Testing
{
    [TestFixture]
    public class StructureMapExceptionTester
    {
        
        private void assertErrorIsThrown(int errorCode, string xml, Action<Container> action)
        {
            var document = new XmlDocument();
            document.LoadXml(xml.Replace("\"", "'"));

            var builder = new PluginGraphBuilder();
            builder.Add(new ConfigurationParser(document.DocumentElement));
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

        [Test, Ignore("Will have to be redone")]
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


        [Test, Ignore("will have to be redone")]
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