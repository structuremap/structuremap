using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    public class GenericsPluginGraph
    {
        private readonly Dictionary<Type, PluginFamily> _families;

        public GenericsPluginGraph()
        {
            _families = new Dictionary<Type, PluginFamily>();
        }

        public static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            try
            {
                return checkGenericType(pluggedType, pluginType);
            }
            catch (Exception e)
            {
                string message =
                    string.Format("Could not Determine Whether Type '{0}' plugs into Type '{1}'",
                                  pluginType.Name,
                                  pluggedType.Name);
                throw new ApplicationException(message, e);
            }
        }

        private static bool checkGenericType(Type pluggedType, Type pluginType)
        {
// check interfaces
            foreach (Type type in pluggedType.GetInterfaces())
            {
                if (!type.IsGenericType)
                {
                    continue;
                }

                if (type.GetGenericTypeDefinition().Equals(pluginType))
                {
                    return true;
                }
            }

            if (pluggedType.BaseType.IsGenericType)
            {
                Type baseType = pluggedType.BaseType.GetGenericTypeDefinition();

                if (baseType.Equals(pluginType))
                {
                    return true;
                }
                else
                {
                    return CanBeCast(pluginType, baseType);
                }
            }

            return false;
        }

        public void AddFamily(PluginFamily family)
        {
            _families.Add(family.PluginType, family);
        }


        public PluginFamily CreateTemplatedFamily(Type templatedType, ProfileManager profileManager)
        {
            Type basicType = templatedType.GetGenericTypeDefinition();

            if (_families.ContainsKey(basicType))
            {
                PluginFamily basicFamily = _families[basicType];
                Type[] templatedParameterTypes = templatedType.GetGenericArguments();

                profileManager.CopyDefaults(basicType, templatedType);

                return CreateTemplatedClone(basicFamily, templatedParameterTypes);
            }
            else
            {
                return null;
            }
        }

        // TODO:  This code sucks.  What's going on here?
        public static PluginFamily CreateTemplatedClone(PluginFamily baseFamily, params Type[] templateTypes)
        {
            Type templatedType = baseFamily.PluginType.MakeGenericType(templateTypes);
            PluginFamily templatedFamily = new PluginFamily(templatedType, baseFamily.Parent);
            templatedFamily.DefaultInstanceKey = baseFamily.DefaultInstanceKey;
            templatedFamily.Policy = baseFamily.Policy.Clone();

            // Add Plugins
            foreach (Plugin plugin in baseFamily.Plugins)
            {
                if (CanBePluggedIntoGenericType(baseFamily.PluginType, plugin.PluggedType, templateTypes))
                {
                    Plugin templatedPlugin = CreateTemplatedClone(plugin, templateTypes);
                    templatedFamily.Plugins.Add(templatedPlugin);
                }
            }

            // TODO -- Got a big problem here.  Intances need to be copied over
            foreach (IDiagnosticInstance instance in baseFamily.GetAllInstances())
            {
                if (instance.CanBePartOfPluginFamily(templatedFamily))
                {
                    templatedFamily.AddInstance((Instance) instance);
                }
            }

            // Need to attach the new PluginFamily to the old PluginGraph
            baseFamily.Parent.PluginFamilies.Add(templatedFamily);

            return templatedFamily;
        }


        public static Plugin CreateTemplatedClone(Plugin plugin, params Type[] types)
        {
            Type templatedType;
            if (plugin.PluggedType.IsGenericType)
            {
                templatedType = plugin.PluggedType.MakeGenericType(types);
            }
            else
            {
                templatedType = plugin.PluggedType;
            }
            Plugin templatedPlugin = new Plugin(templatedType, plugin.ConcreteKey);
            foreach (SetterProperty setter in plugin.Setters)
            {
                templatedPlugin.Setters.Add(setter.Name);
            }

            return templatedPlugin;
        }


        public static bool CanBePluggedIntoGenericType(Type pluginType, Type pluggedType, params Type[] templateTypes)
        {
            bool isValid = true;

            Type interfaceType = pluggedType.GetInterface(pluginType.Name);
            if (interfaceType == null)
            {
                interfaceType = pluggedType.BaseType;
            }

            Type[] pluginArgs = pluginType.GetGenericArguments();
            Type[] pluggableArgs = interfaceType.GetGenericArguments();

            if (templateTypes.Length != pluginArgs.Length &&
                pluginArgs.Length != pluggableArgs.Length)
            {
                return false;
            }

            for (int i = 0; i < templateTypes.Length; i++)
            {
                isValid &= templateTypes[i] == pluggableArgs[i] ||
                           pluginArgs[i].IsGenericParameter &&
                           pluggableArgs[i].IsGenericParameter;
            }
            return isValid;
        }
    }
}