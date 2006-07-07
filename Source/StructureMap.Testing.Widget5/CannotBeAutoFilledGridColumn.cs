using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
	[Pluggable("CannotBeAutoFilled")]
	public class CannotBeAutoFilledGridColumn
	{
		private readonly IWidget _widget;
		private Rule _rule;
		private string _name;

		public CannotBeAutoFilledGridColumn(IWidget widget)
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

		[SetterProperty]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}