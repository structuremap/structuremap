using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StructureMap.Pipeline
{
    public class ConstructorSelector
    {
        private readonly IList<IConstructorSelector> _selectors = new List<IConstructorSelector>();

        private readonly IConstructorSelector[] _defaults = new IConstructorSelector[]
        {
            new AttributeConstructorSelector(),
            new GreediestConstructorSelector()
        };

        public ConstructorSelector()
        {
        }

        public void Add(IConstructorSelector selector)
        {
            _selectors.Add(selector);
        }

        public ConstructorInfo Select(Type pluggedType)
        {
            return _selectors.Union(_defaults).FirstValue(x => x.Find(pluggedType));
        }
    }
}