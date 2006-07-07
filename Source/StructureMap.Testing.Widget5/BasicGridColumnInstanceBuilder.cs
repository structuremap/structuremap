using System;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
	/// <summary>
	/// Used just to generate the template for IL generation
	/// </summary>
	public class BasicGridColumnInstanceBuilder : InstanceBuilder
	{
		public BasicGridColumnInstanceBuilder() : base()
		{
		}


		public override string PluginType
		{
			get { throw new NotImplementedException(); }
		}

		public override string PluggedType
		{
			get { throw new NotImplementedException(); }
		}

		public override string ConcreteTypeKey
		{
			get { throw new NotImplementedException(); }
		}

		public override object BuildInstance(InstanceMemento Memento)
		{
			BasicGridColumn column = new BasicGridColumn(Memento.GetProperty("headerText"));

//			column.Widget = 
//				(IWidget) Memento.GetChild("Widget", "StructureMap.Testing.Widget.IWidget", this.Manager);
//
//			column.FontStyle = 
//				(FontStyleEnum) Enum.Parse( typeof ( FontStyleEnum ), Memento.GetProperty( "FontStyle" ), true );

//			column.ColumnName = Memento.GetProperty("ColumnName");

			column.Rules =
				(Rule[]) this.Manager.CreateInstanceArray("StructureMap.Testing.Widget.Rule", Memento.GetChildrenArray("Rules"));

//
//			column.WrapLines = bool.Parse(Memento.GetProperty("WrapLines"));

			return column;
		}
	}
}