using System;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Widget
{
    public class Decision
    {
        public Rule[] Rules;

        public Decision(Rule[] Rules)
        {
            this.Rules = Rules;
        }
    }


    public class DecisionBuilder : InstanceBuilder
    {
        public override string ConcreteTypeKey
        {
            get { return null; }
        }

        public override object BuildInstance(IConfiguredInstance instance, StructureMap.Pipeline.IBuildSession session)
        {
            return new Decision(
                (Rule[]) session.CreateInstanceArray(typeof(Rule), instance.GetChildrenArray("Rules")));
        }


        public override Type PluggedType
        {
            get { return typeof (Decision); }
        }
    }
}