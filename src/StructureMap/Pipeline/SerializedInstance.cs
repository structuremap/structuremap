using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class SerializedInstance : ExpressedInstance<SerializedInstance>
    {
        private readonly object _locker = new object();
        private readonly MemoryStream _stream;

        public SerializedInstance(object template)
        {
            _stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(_stream, template);
        }

        protected override SerializedInstance thisInstance { get { return this; } }

        protected override string getDescription()
        {
            return "Serialized instance";
        }

        protected override object build(Type pluginType, IBuildSession session)
        {
            lock (_locker)
            {
                _stream.Position = 0;
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(_stream);
            }
        }
    }
}