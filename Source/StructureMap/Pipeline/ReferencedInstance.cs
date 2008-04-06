using System;

namespace StructureMap.Pipeline
{
    public class ReferencedInstance : Instance
    {
        private readonly string _referenceKey;


        public ReferencedInstance(string referenceKey)
        {
            // TODO:  VALIDATION if referenceKey is null or empty
            _referenceKey = referenceKey;
        }


        protected override object build(Type type, IInstanceCreator creator)
        {
            return creator.CreateInstance(type, _referenceKey);
        }
    }
}