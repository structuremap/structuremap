using System;

namespace StructureMap.Configuration.Tokens.Properties
{
    public interface IProperty
    {
        string PropertyName { get; }

        Type PropertyType { get; }

        void Validate(IInstanceValidator validator);
    }
}