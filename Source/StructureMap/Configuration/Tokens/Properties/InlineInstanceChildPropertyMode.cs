using System;

namespace StructureMap.Configuration.Tokens.Properties
{
    [Serializable]
    public class InlineInstanceChildPropertyMode : IChildPropertyMode
    {
        private readonly ChildProperty _property;

        public InlineInstanceChildPropertyMode(ChildProperty property)
        {
            _property = property;
        }

        public void Validate(IInstanceValidator validator)
        {
            _property.InnerInstance.Validate(validator);
        }

        public void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleInlineChildProperty(_property);
        }
    }
}