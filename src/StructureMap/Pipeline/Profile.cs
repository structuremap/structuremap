using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public class Profile
    {
        private readonly string _name;
        private Dictionary<Type, Instance> _instances = new Dictionary<Type, Instance>();

        public Profile(string name)
        {
            _name = name;
        }


        public string Name { get { return _name; } }

        public void SetDefault(Type pluginType, Instance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (_instances.ContainsKey(pluginType))
            {
                _instances[pluginType] = instance;
            }
            else
            {
                _instances.Add(pluginType, instance);
            }
        }

        public Instance GetDefault(Type pluginType)
        {
            if (_instances.ContainsKey(pluginType))
            {
                return _instances[pluginType];
            }

            return null;
        }

        public void FillTypeInto(Type pluginType, Instance instance)
        {
            if (!_instances.ContainsKey(pluginType))
            {
                _instances.Add(pluginType, instance);
            }
        }

        public void FillAllTypesInto(Profile destination)
        {
            foreach (var pair in _instances)
            {
                destination.FillTypeInto(pair.Key, pair.Value);
            }
        }

        public void Merge(Profile destination)
        {
            foreach (var pair in _instances)
            {
                destination.SetDefault(pair.Key, pair.Value);
            }
        }

        public void FindMasterInstances(PluginGraph graph)
        {
            var master = new Dictionary<Type, Instance>();

            foreach (var pair in _instances)
            {
                PluginFamily family = graph.Families[pair.Key];
                Instance masterInstance = ((IDiagnosticInstance) pair.Value)
                    .FindInstanceForProfile(family, _name, graph.Log);

                master.Add(pair.Key, masterInstance);
            }

            _instances = master;
        }

        public static string InstanceKeyForProfile(string profileName)
        {
            return "Default Instance for Profile " + profileName;
        }

        public void CopyDefault(Type sourceType, Type destinationType, PluginFamily family)
        {
            if (!_instances.ContainsKey(sourceType)) return;

            Instance sourceInstance = _instances[sourceType];
            if (sourceInstance.IsReference)
            {
                _instances.Add(destinationType, sourceInstance);
            }
            else
            {
                family.ForInstance(sourceInstance.Name, x => { _instances.Add(destinationType, x); });
            }
        }


        public void Remove(Type pluginType)
        {
            _instances.Remove(pluginType);
        }

        public void Clear()
        {
            _instances.Clear();
        }

        public void Remove(Type pluginType, Instance instance)
        {
            if (!_instances.ContainsKey(pluginType)) return;

            if (_instances[pluginType] == instance)
            {
                _instances.Remove(pluginType);
            }
        }
    }
}