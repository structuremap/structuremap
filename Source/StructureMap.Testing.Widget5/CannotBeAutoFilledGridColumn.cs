using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    [Pluggable("CannotBeAutoFilled")]
    public class CannotBeAutoFilledGridColumn
    {
        private readonly IWidget _widget;

        public CannotBeAutoFilledGridColumn(IWidget widget)
        {
            _widget = widget;
        }

        public IWidget Widget
        {
            get { return _widget; }
        }

        [SetterProperty]
        public Rule Rule { get; set; }

        [SetterProperty]
        public string Name { get; set; }
    }
}