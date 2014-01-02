using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    public class ArrayDependencySource : IDependencySource
    {
        private readonly Type _itemType;
        private readonly List<IDependencySource> _items = new List<IDependencySource>();

        public ArrayDependencySource(Type itemType, params IDependencySource[] items)
        {
            _itemType = itemType;
            _items.AddRange(items);
        }

        public Type ItemType
        {
            get { return _itemType; }
        }

        public void Add(IDependencySource item)
        {
            _items.Add(item);
        }

        public IEnumerable<IDependencySource> Items
        {
            get { return _items; }
        }

        public string Description { get; private set; }

        public virtual Expression ToExpression(ParameterExpression session)
        {
            return Expression.NewArrayInit(_itemType, _items.Select(x => x.ToExpression(session)));
        }
    }
}