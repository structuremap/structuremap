using System;
using System.Collections.Generic;
using System.Reflection;

namespace StructureMap.Pipeline
{
    public class ConstructorSelector
    {
        private readonly Stack<IConstructorSelector> _selectors = new Stack<IConstructorSelector>();

        public ConstructorSelector()
        {
            _selectors.Push(new GreediestConstructorSelector());
            _selectors.Push(new AttributeConstructorSelector());
        }

        public ConstructorInfo Select(Type pluggedType)
        {
            return _selectors.FirstValue(x => x.Find(pluggedType));
        }
    }
}