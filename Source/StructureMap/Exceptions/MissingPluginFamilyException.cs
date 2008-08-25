using System;
using System.Runtime.Serialization;

namespace StructureMap.Exceptions
{
    [Serializable]
    public class MissingPluginFamilyException : ApplicationException
    {
        private string _message;

        public MissingPluginFamilyException(string pluginTypeName) : base()
        {
            _message = string.Format("Type {0} is not a configured PluginFamily", pluginTypeName);
        }

        public override string Message
        {
            get { return _message; }
        }

        protected MissingPluginFamilyException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            _message = info.GetString("message");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("message", _message, typeof(string));

            base.GetObjectData(info, context);
        }
    }
}