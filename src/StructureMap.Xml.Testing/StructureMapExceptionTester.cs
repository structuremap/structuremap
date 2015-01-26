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
        
        private StructureMapException assertErrorIsThrown(string xml, Action<Container> action)
        {
            var document = new XmlDocument();
            document.LoadXml(xml.Replace("\"", "'"));

            var builder = new PluginGraphBuilder();
            builder.Add(new ConfigurationParser(document.DocumentElement));
            var container = new Container(builder.Build());

            return Exception<StructureMapException>.ShouldBeThrownBy(() => {
                action(container);
            });
            
        }


        [Test, Ignore("Will have to be redone")]
        public void CouldNotFindInstanceKey()
        {
            assertErrorIsThrown(@"
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
            assertErrorIsThrown(@"
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