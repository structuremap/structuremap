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


        public string ReferenceKey
        {
            get { return _referenceKey; }
        }

        protected override object build(Type pluginType, IInstanceCreator creator)
        {
            return creator.CreateInstance(pluginType, _referenceKey);
        }
    }
}