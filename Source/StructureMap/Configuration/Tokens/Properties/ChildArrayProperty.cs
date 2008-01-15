using System;
using System.Collections;
using StructureMap.Graph;

namespace StructureMap.Configuration.Tokens.Properties
{
    [Serializable]
    public class ChildArrayProperty : Property
    {
        private ArrayList _innerProperties = new ArrayList();
        private string _propertyTypeName;

        public ChildArrayProperty(PropertyDefinition definition, InstanceMemento memento, PluginGraphReport report)
            : base(definition)
        {
            try
            {
                _propertyTypeName = TypePath.GetAssemblyQualifiedName(definition.PropertyType);
                InstanceMemento[] children = memento.GetChildrenArray(definition.PropertyName);
                processChildren(children, definition, report);
            }
            catch (Exception ex)
            {
                Problem problem = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, ex);
                LogProblem(problem);
            }
        }


        public ChildProperty[] InnerProperties
        {
            get { return (ChildProperty[]) _innerProperties.ToArray(typeof (ChildProperty)); }
        }

        public int Count
        {
            get { return _innerProperties.Count; }
        }

        public ChildProperty this[int index]
        {
            get { return (ChildProperty) _innerProperties[index]; }
        }

        public override GraphObject[] Children
        {
            get { return InnerProperties; }
        }

        public string PropertyTypeName
        {
            get { return _propertyTypeName; }
            set { _propertyTypeName = value; }
        }

        private void processChildren(InstanceMemento[] children, PropertyDefinition definition, PluginGraphReport report)
        {
            if (children == null)
            {
                string message = string.Format(
                    "Could not find the array of child InstanceMemento's for the property {0}",
                    definition.PropertyName);

                Problem problem = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, message);
                LogProblem(problem);
            }
            else
            {
                buildChildProperties(children, definition, report);
            }
        }

        private void buildChildProperties(InstanceMemento[] children, PropertyDefinition definition,
                                          PluginGraphReport report)
        {
            int index = 0;
            foreach (InstanceMemento childMemento in children)
            {
                ChildProperty child = ChildProperty.BuildArrayChild(definition, childMemento, report);
                child.ArrayIndex = ++index;
                _innerProperties.Add(child);
            }
        }


        public void AddChildProperty(ChildProperty child)
        {
            _innerProperties.Add(child);
        }

        public override void Validate(IInstanceValidator validator)
        {
            foreach (ChildProperty childProperty in _innerProperties)
            {
                childProperty.Validate(validator);
            }
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleChildArrayProperty(this);
        }
    }
}