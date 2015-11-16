using System;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    // SAMPLE: EnumerableFamilyPolicy
    public class EnumerableFamilyPolicy : IFamilyPolicy
    {
        public PluginFamily Build(Type type)
        {
            if (EnumerableInstance.IsEnumerable(type))
            {
                var family = new PluginFamily(type);
                family.SetDefault(new AllPossibleInstance(type));

                return family;
            }

            return null;
        }

        public bool AppliesToHasFamilyChecks
        {
            get
            {
                return false;
            }
            
        }
    }
    // ENDSAMPLE

    public class AllPossibleInstance : Instance
    {
        private readonly Type _returnedType;

        public AllPossibleInstance(Type returnedType)
        {
            _returnedType = returnedType;
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new AllPossibleValuesDependencySource(_returnedType);
        }

        public override string Description
        {
            get { return "All registered children for " + _returnedType.GetFullName(); }
        }

        public override Type ReturnedType
        {
            get { return _returnedType; }
        }
    }
}