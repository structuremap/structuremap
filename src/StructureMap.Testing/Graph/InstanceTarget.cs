using System;

namespace StructureMap.Testing.Graph
{
    //[Pluggable("Default")]
    public class InstanceTarget
    {
        private readonly bool _isValid;
        private readonly string _name;
        private readonly int _quantity;

        public InstanceTarget(string name, bool isValid, int quantity)
        {
            _name = name;
            _isValid = isValid;
            _quantity = quantity;

            if (quantity < 0)
            {
                throw new ApplicationException("cannot be negative");
            }
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        [ValidationMethod]
        public void Validate()
        {
            if (!IsValid)
            {
                throw new ApplicationException("not valid.");
            }
        }
    }
}