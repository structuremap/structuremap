using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Building
{
    public class FakeSession : IBuildSession
    {
        public object BuildNewInSession(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public object BuildNewInOriginalContext(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public object ResolveFromLifecycle(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public string RequestedName { get; set; }
    }
}