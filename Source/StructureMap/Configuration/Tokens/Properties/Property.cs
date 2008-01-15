using System;

namespace StructureMap.Configuration.Tokens.Properties
{
    [Serializable]
    public class Property : GraphObject, IProperty
    {
        private readonly PropertyDefinition _definition;

        public Property(PropertyDefinition definition)
        {
            _definition = definition;
        }

        public PropertyDefinition Definition
        {
            get { return _definition; }
        }

        protected override string key
        {
            get { return PropertyName; }
        }

        #region IProperty Members

        public virtual string PropertyName
        {
            get { return _definition.PropertyName; }
        }

        public Type PropertyType
        {
            get { return _definition.PropertyType; }
        }

        public virtual void Validate(IInstanceValidator validator)
        {
            // no-op
        }

        #endregion
    }
}