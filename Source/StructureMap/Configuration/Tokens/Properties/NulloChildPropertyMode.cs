using System;

namespace StructureMap.Configuration.Tokens.Properties
{
    [Serializable]
    public class NulloChildPropertyMode : IChildPropertyMode
    {
        private readonly ChildProperty _property;

        public NulloChildPropertyMode(ChildProperty property)
        {
            _property = property;
        }

        #region IChildPropertyMode Members

        public void Validate(IInstanceValidator validator)
        {
            // no-op
        }

        public void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleNotDefinedChildProperty(_property);
        }

        #endregion
    }
}