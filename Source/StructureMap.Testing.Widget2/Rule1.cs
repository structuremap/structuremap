using System;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget2
{
    [Pluggable("Rule1", "")]
    public class Rule1 : Rule
    {
        public Rule1()
        {
        }

        [ValidationMethod]
        public void Validate()
        {
            throw new ApplicationException("I don't like this");
        }
    }
}