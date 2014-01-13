using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using StructureMap.Building;

namespace StructureMap.Configuration.Xml
{
    public class StructureMapConfigurationSection : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
            var allNodes = parent as IList<XmlNode>;
            if (allNodes == null)
            {
                allNodes = new List<XmlNode>();
            }
            allNodes.Add(section);
            return allNodes;
        }

        #endregion

        public static IList<XmlNode> GetStructureMapConfiguration()
        {
            var nodes = ConfigurationSettings.GetConfig(XmlConstants.STRUCTUREMAP) as IList<XmlNode>;
            if (nodes == null)
            {
                // TODO -- make sure there's a UT on this behavior
                throw new StructureMapConfigurationException("The <{0}> section could not be loaded from the application configuration file.", XmlConstants.STRUCTUREMAP);
            }
            return nodes;
        }
    }
}