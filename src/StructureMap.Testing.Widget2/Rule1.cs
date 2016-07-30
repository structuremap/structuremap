using System;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget2
{
    public class Rule1 : Rule
    {
        [ValidationMethod]
        public void Validate()
        {
            throw new Exception("I don't like this");
        }
    }
}