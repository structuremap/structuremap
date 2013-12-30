using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Attributes;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class SetterRules
    {
        private readonly List<Func<PropertyInfo, bool>> _setterRules = new List<Func<PropertyInfo, bool>>();

        public SetterRules()
        {
            Add(p => p.HasAttribute<SetterPropertyAttribute>());
        }

        public void Configure(Plugin plugin)
        {
            foreach (var rule in _setterRules)
            {
                plugin.UseSetterRule(rule);
            }
        }

        public void Clear()
        {
            _setterRules.Clear();
        }

        public void Add(Func<PropertyInfo, bool> predicate)
        {
            _setterRules.Add(predicate);
        }


        public void Add(IEnumerable<Func<PropertyInfo, bool>> rules)
        {
            _setterRules.AddRange(rules);
        }
    }
}