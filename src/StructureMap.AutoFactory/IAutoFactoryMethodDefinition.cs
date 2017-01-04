using StructureMap.Pipeline;
using System;

namespace StructureMap.AutoFactory
{
    // SAMPLE: IAutoFactoryMethodDefinition
    /// <summary>
    /// Describes how AutoFactory should treat the specific method declared in an abstract factory interface.
    /// </summary>
    public interface IAutoFactoryMethodDefinition
    {
        /// <summary>
        /// The method type. See <see cref="AutoFactoryMethodType"/> for possible values.
        /// </summary>
        AutoFactoryMethodType MethodType { get; }

        /// <summary>
        /// The instance type to create.
        /// </summary>
        Type InstanceType { get; }

        /// <summary>
        /// The instance name if available.
        /// </summary>
        string InstanceName { get; }

        /// <summary>
        /// Explicit arguments if available.
        /// </summary>
        ExplicitArguments ExplicitArguments { get; }
    }

    // ENDSAMPLE
}