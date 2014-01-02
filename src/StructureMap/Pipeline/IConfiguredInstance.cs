using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public enum CannotFindProperty
    {
        ThrowException,
        Ignore
    }

    // TODO -- let's see if we can thin this down
    public interface IConfiguredInstance
    {
        string Name { get; }
        Type PluggedType { get; }

        DependencyCollection Dependencies { get; }
    }
}