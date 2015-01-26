using System;
using FubuMVC.Core;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuMVC.StructureMap3
{
    public static class BootstrappingExtensions
    {
        public static FubuApplication StructureMapObjectFactory(this IContainerFacilityExpression expression)
        {
            return expression.StructureMap(() => ObjectFactory.Container);
        } 

        public static FubuApplication StructureMapObjectFactory(this IContainerFacilityExpression expression, Action<IInitializationExpression> structureMapBootstrapper)
        {
            return expression.StructureMap(() =>
            {
                ObjectFactory.Initialize(structureMapBootstrapper);
                return ObjectFactory.Container;
            });
        }

        /// <summary>
        /// Apply a new StructureMap container for the application using the specified StructureMap Registry
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static FubuApplication StructureMap<T>(this IContainerFacilityExpression expression)
            where T : Registry, new()
        {
            return expression.StructureMap(() => new Container(new T()));
        }

        /// <summary>
        /// Applies a new StructureMap container to the FubuMVC application
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static FubuApplication StructureMap(this IContainerFacilityExpression expression)
        {
            return expression.StructureMap(new Container());
        }

        /// <summary>
        /// Applies the given StructureMap container as the root container for this FubuMVC application
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static FubuApplication StructureMap(this IContainerFacilityExpression expression, IContainer container)
        {
            return expression.StructureMap(() => container);
        }

        /// <summary>
        /// Uses the StructureMap container built by the specified lambda as the application container. Use this overload if the StructureMap registration
        /// depends on the Bottle scanning (PackageRegistry.PackageAssemblies).
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="createContainer"></param>
        /// <returns></returns>
        public static FubuApplication StructureMap(this IContainerFacilityExpression expression, Func<IContainer> createContainer)
        {
            return expression.ContainerFacility(() =>
            {
                var container = createContainer();

                return new StructureMapContainerFacility(container);
            });
        }
    }
}