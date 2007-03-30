

using System.Configuration;
using System.Diagnostics;
using System.Xml;

namespace StructureMap.Configuration
{
    public class StructureMapConfigurationSection : IConfigurationSectionHandler
    {
        private XmlNode _node;

        public object Create(object parent, object configContext, XmlNode section)
        {
            return _node;
        }

        public static XmlNode GetStructureMapConfiguration()
        {
            object config = ConfigurationManager.GetSection(XmlConstants.STRUCTUREMAP);
            

            return null;
            /*
            object o = ConfigurationSettings.GetConfig(XmlConstants.STRUCTUREMAP);
            XmlNode node = o as XmlNode;
            if (node == null)
            {
                throw new ApplicationException("No <StructureMap> section was found in the application config file");
            }
            return node;
             */
        }
    }
}
