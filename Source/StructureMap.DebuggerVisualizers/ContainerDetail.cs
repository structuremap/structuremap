using System;
using System.Collections.Generic;

namespace StructureMap.DebuggerVisualizers
{
    [Serializable]
    public class ContainerDetail
    {
        private readonly string[] _sources;
        private readonly PluginTypeDetail[] _pluginTypeDetails;

        public ContainerDetail(string[] sources, PluginTypeDetail[] types)
        {
            _sources = sources;
            _pluginTypeDetails = types;
        }

        public string[] Sources
        {
            get { return _sources; }
        }

        public PluginTypeDetail[] PluginTypes
        {
            get { return _pluginTypeDetails; }
        }
    }

    [Serializable]
    public class PluginTypeDetail
    {
        private readonly Type _type;
        private readonly Type _buildPolicyType;
        private readonly InstanceDetail[] instanceDetails;
        private readonly IList<InstanceDetail> _instances = new List<InstanceDetail>();

        public PluginTypeDetail(Type type, Type buildPolicyType, InstanceDetail[] instanceDetails)
        {
            _type = type;
            _buildPolicyType = buildPolicyType;
            this.instanceDetails = instanceDetails;
        }

        public InstanceDetail[] Instances
        {
            get { return instanceDetails; }
        }

        public Type BuildPolicy
        {
            get { return _buildPolicyType; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }

    [Serializable]
    public class InstanceDetail
    {
        private readonly string _name;
        private readonly string _description;
        private readonly Type _concreteType;

        public InstanceDetail(string name, string description, Type concreteType)
        {
            _name = name;
            _description = description;
            _concreteType = concreteType;
        }

        public Type ConcreteType
        {
            get { return _concreteType; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Name
        {
            get { return _name; }
        }
    }

}