using System;
using System.Linq;
using System.Text;
using System.Xml;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public interface ITypeReader
    {
        bool CanProcess(Type pluginType);
        Instance Read(XmlNode node, Type pluginType);
    }
}
