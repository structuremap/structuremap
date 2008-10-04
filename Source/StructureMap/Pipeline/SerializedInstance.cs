using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StructureMap.Pipeline
{
    public class SerializedInstance : ExpressedInstance<SerializedInstance>
    {
        private MemoryStream _stream;
        private object _locker = new object();

        public SerializedInstance(object template)
        {
            _stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_stream, template);
        }

        protected override string getDescription()
        {
            return "Serialized instance";
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            lock (_locker)
            {
                _stream.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(_stream);
            }
        }

        protected override SerializedInstance thisInstance
        {
            get { return this; }
        }
    }
}
