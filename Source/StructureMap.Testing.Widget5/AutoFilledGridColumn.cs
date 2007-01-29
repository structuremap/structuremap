using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    [Pluggable("AutoFilled")]
    public class AutoFilledGridColumn
    {
        private readonly IWidget _widget;
        private Rule _rule;

        public AutoFilledGridColumn(IWidget widget)
        {
            _widget = widget;
        }

        public IWidget Widget
        {
            get { return _widget; }
        }

        [SetterProperty]
        public Rule Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }
    }
}