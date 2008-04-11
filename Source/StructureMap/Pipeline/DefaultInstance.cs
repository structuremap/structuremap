using System;
using System.Collections.Generic;
using System.Text;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        protected override object build(Type pluginType, IInstanceCreator creator)
        {
            return creator.CreateInstance(pluginType);
        }
    }
}
