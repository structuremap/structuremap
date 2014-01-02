using System;
using System.Runtime.Serialization;

namespace StructureMap.Configuration
{
    [Serializable]
    public class MalformedAliasException : Exception
    {
        public MalformedAliasException(string xml)
            : base(
                "{0} is a malformed alias.  Should be of form <Alias Key=\"key\" Type=\"assembly qualified name\" />"
                    .ToFormat(xml))
        {
        }


        protected MalformedAliasException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}