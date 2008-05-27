using System;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    public class RegistryExpressions
    {
        /// <summary>
        /// Convenience method to start the definition of an instance of type T
        /// </summary>
        /// <typeparam name="PLUGGEDTYPE"></typeparam>
        /// <returns></returns>
        public static ConfiguredInstance Instance<PLUGGEDTYPE>()
        {
            ConfiguredInstance instance = new ConfiguredInstance(typeof (PLUGGEDTYPE));

            return instance;
        }

        /// <summary>
        /// Convenience method to register a prototype instance
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public static PrototypeInstance Prototype<PLUGINTYPE>(PLUGINTYPE prototype)
        {
            return new PrototypeInstance((ICloneable) prototype);
        }

        /// <summary>
        /// Convenience method to register a preconfigured instance of type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static LiteralInstance Object<PLUGINTYPE>(PLUGINTYPE instance)
        {
            return new LiteralInstance(instance);
        }

        /// <summary>
        /// Registers a UserControl as an instance 
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public UserControlInstance LoadControlFromUrl(string url)
        {
            return new UserControlInstance(url);
        }

        public static ConstructorInstance ConstructedBy<PLUGINTYPE>
            (Func<PLUGINTYPE> builder)
        {
            return new ConstructorInstance(delegate() { return builder(); });
        }

        public static ReferencedInstance Instance(string referencedKey)
        {
            return new ReferencedInstance(referencedKey);
        }
    }
}