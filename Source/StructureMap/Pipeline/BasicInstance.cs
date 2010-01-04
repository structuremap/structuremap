using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public interface BasicInstance
    {
        Type PluggedType { get; }


        Dictionary<string, string> Properties { get; }
        Dictionary<string, Instance> Children { get; }
        Dictionary<string, Instance[]> Arrays { get; }
    }
}