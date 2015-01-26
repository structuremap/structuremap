using System;

namespace StructureMap.Configuration
{
    public class MalformedAliasException : Exception
    {
        public MalformedAliasException(string xml)
            : base(
                "{0} is a malformed alias.  Should be of form <Alias Key=\"key\" Type=\"assembly qualified name\" />"
                    .ToFormat(xml))
        {
        }
    }
}