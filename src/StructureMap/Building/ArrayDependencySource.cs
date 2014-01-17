using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StructureMap.TypeRules;

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

        public virtual string Description
        {
            get
            {
                return "Array of all possible {0} values".ToFormat(_itemType.GetFullName());
            }
        }

        public virtual Expression ToExpression(ParameterExpression session)
        {
            return Expression.NewArrayInit(_itemType, _items.Select(x => x.ToExpression(session)));
        }

        public Type ReturnedType
        {
            get
            {
                return _itemType.MakeArrayType();
            }
            
        }
    }
}