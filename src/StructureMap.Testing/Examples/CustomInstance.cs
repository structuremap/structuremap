using System;
using System.IO;
using System.Xml.Serialization;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Examples
{
    public class XmlFileInstance<T> : Instance
    {
        private readonly string _fileName;

        public XmlFileInstance(string fileName)
        {
            _fileName = fileName;
        }

        protected override string getDescription()
        {
            return "Xml Serialized {0} at {1}".ToFormat(typeof (T).FullName, _fileName);
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            var serializer = new XmlSerializer(typeof (T));
            using (var stream = new FileStream(_fileName, FileMode.Open))
            {
                return serializer.Deserialize(stream);
            }
        }
    }

    public class Address
    {
    }

    public class CustomInstanceRegistry : Registry
    {
        public CustomInstanceRegistry()
        {
            For<Address>().TheDefault.IsThis(new XmlFileInstance<Address>("address1.xml"));
        }
    }
}