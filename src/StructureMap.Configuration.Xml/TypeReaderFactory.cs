using System;
using System.Collections.Generic;
using System.Xml;

namespace StructureMap.Configuration.Xml
{
    public static class TypeReaderFactory
    {
        private static readonly List<ITypeReader<XmlNode>> _readers = new List<ITypeReader<XmlNode>>();

        static TypeReaderFactory()
        {
            _readers.Add(new DictionaryReader());
            _readers.Add(new PrimitiveArrayReader());
        }

        public static ITypeReader<XmlNode> GetReader(Type pluginType)
        {
            foreach (var reader in _readers)
            {
                if (reader.CanProcess(pluginType))
                {
                    return reader;
                }
            }

            return null;
        }
    }
}