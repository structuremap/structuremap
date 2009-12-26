using System;
using System.Collections.Generic;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public class GenericsPluginGraph
    {
        private Cache<Type, PluginFamily> _families;

        public GenericsPluginGraph()
        {
            _families = new Cache<Type, PluginFamily>(pluginType => new PluginFamily(pluginType));
        }

        public int FamilyCount { get { return _families.Count; } }

        public IEnumerable<PluginTypeConfiguration> Families
        {
            get
            {
                foreach (PluginFamily family in _families.GetAll())
                {
                    yield return family.GetConfiguration();
                }
            }
        }

        public GenericsPluginGraph Clone()
        {
            return new GenericsPluginGraph
            {
                _families = _families.Clone()
            };
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
            _families[family.PluginType] = family;
        }


        public PluginFamily CreateTemplatedFamily(Type templatedType, ProfileManager profileManager)
        {
            Type basicType = templatedType.GetGenericTypeDefinition();

            if (_families.Has(basicType))
            {
                PluginFamily basicFamily = _families[basicType];
                Type[] templatedParameterTypes = templatedType.GetGenericArguments();


                PluginFamily templatedFamily = basicFamily.CreateTemplatedClone(templatedParameterTypes);
                PluginFamily family = templatedFamily;
                profileManager.CopyDefaults(basicType, templatedType, family);

                return family;
            }
            else
            {
                return null;
            }
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

        public void ImportFrom(GenericsPluginGraph source)
        {
            source._families.Each(ImportFrom);
        }

        public void ImportFrom(PluginFamily sourceFamily)
        {
            PluginFamily destinationFamily = FindFamily(sourceFamily.PluginType);
            destinationFamily.ImportFrom(sourceFamily);
        }

        public PluginFamily FindFamily(Type pluginType)
        {
            return _families[pluginType];
        }

        public bool HasFamily(Type pluginType)
        {
            return _families.Has(pluginType);
        }

        public void ClearAll()
        {
            _families.Clear();
        }
    }
}