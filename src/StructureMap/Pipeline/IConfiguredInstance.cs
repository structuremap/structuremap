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

        object Get(string propertyName, Type pluginType, BuildSession buildSession);

        T Get<T>(string propertyName, BuildSession session);

        bool HasProperty(string propertyName, BuildSession session);

        void SetChild(string name, Instance instance);
        void SetValue(Type type, object value, CannotFindProperty cannotFind);
        void SetValue(string name, object value);

        string GetProperty(string propertyName);

        DependencyCollection Dependencies { get; }
    }
}