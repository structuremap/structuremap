namespace StructureMap.Testing.Widget
{
    [Pluggable("Default")]
    public class Decision
    {
        public Rule[] Rules;

        public Decision(Rule[] Rules)
        {
            this.Rules = Rules;
        }
    }
}