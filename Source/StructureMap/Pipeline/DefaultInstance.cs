using System;
using System.Collections.Generic;
using System.Text;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        protected override object build(Type type, IInstanceCreator creator)
        {
            return creator.CreateInstance(type);
        }
    }
}
