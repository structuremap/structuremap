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

        public override string PluggedType
        {
            get { return null; }
        }


        public override string PluginType
        {
            get { return null; }
        }

        public override object BuildInstance(IConfiguredInstance instance)
        {
            return new Decision(
                (Rule[]) Manager.CreateInstanceArray("StructureMap.Testing.Widget", instance.GetChildrenArray("Rules")));
        }
    }
}