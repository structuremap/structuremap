using System;
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