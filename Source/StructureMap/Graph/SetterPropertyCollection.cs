using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace StructureMap.Graph
{
    /// <summary>
    /// Custom collection class for SetterProperty objects
    /// </summary>
    public class SetterPropertyCollection : IEnumerable<SetterProperty>
    {
        private readonly Plugin _plugin;
        private readonly List<SetterProperty> _properties;

        public SetterPropertyCollection(Plugin plugin)
        {
            _properties = new List<SetterProperty>();
            _plugin = plugin;


            foreach (PropertyInfo property in plugin.PluggedType.GetProperties())
            {
                if (property.CanWrite && property.GetSetMethod(false) != null)
                {
                    var setter = new SetterProperty(property);
                    _properties.Add(setter);
                }
            }
        }

        public int MandatoryCount
        {
            get { return _properties.FindAll(p => p.IsMandatory).Count; }
        }

        public int OptionalCount
        {
            get { return _properties.FindAll(p => !p.IsMandatory).Count; }
        }

        #region IEnumerable<SetterProperty> Members

        IEnumerator<SetterProperty> IEnumerable<SetterProperty>.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<SetterProperty>) this).GetEnumerator();
        }

        #endregion

        public SetterProperty MarkSetterAsMandatory(string propertyName)
        {
            SetterProperty setter = _properties.Find(p => p.Property.Name == propertyName);
            if (setter == null)
            {
                throw new StructureMapException(240, propertyName, _plugin.PluggedType);
            }


            setter.IsMandatory = true;

            return setter;
        }


        public bool IsMandatory(string propertyName)
        {
            SetterProperty property = _properties.Find(p => p.Name == propertyName);
            if (property == null)
            {
                return false;
            }

            return property.IsMandatory;
        }

        public void Merge(SetterPropertyCollection setters)
        {
            foreach (SetterProperty setter in setters)
            {
                if (!IsMandatory(setter.Name))
                {
                    MarkSetterAsMandatory(setter.Name);
                }
            }
        }

        public void Visit(IArgumentVisitor visitor)
        {
            foreach (SetterProperty setter in this)
            {
                setter.Visit(visitor);
            }
        }

        public bool CanBeAutoFilled()
        {
            bool returnValue = true;

            foreach (SetterProperty setterProperty in this)
            {
                if (setterProperty.IsMandatory)
                {
                    returnValue = returnValue && setterProperty.CanBeAutoFilled;
                }
            }

            return returnValue;
        }

        public string FindFirstConstructorArgumentOfType(Type type)
        {
            foreach (SetterProperty setterProperty in this)
            {
                if (setterProperty.Property.PropertyType.Equals(type))
                {
                    return setterProperty.Name;
                }
            }

            return null;
        }

        public void UseSetterRule(Predicate<PropertyInfo> rule)
        {
            _properties.FindAll(setter => rule(setter.Property)).ForEach(setter => setter.IsMandatory = true);
        }
    }
}