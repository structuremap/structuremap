using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class AutoFilledGridColumn
    {
        private readonly IWidget _widget;

        public AutoFilledGridColumn(IWidget widget)
        {
            _widget = widget;
        }

        public IWidget Widget { get { return _widget; } }

        [SetterProperty]
        public Rule Rule { get; set; }
    }
}