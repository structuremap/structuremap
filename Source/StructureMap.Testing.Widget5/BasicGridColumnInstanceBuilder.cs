using System;
using StructureMap.Pipeline;
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


        public override string ConcreteTypeKey
        {
            get { throw new NotImplementedException(); }
        }

        public override Type PluggedType
        {
            get { throw new NotImplementedException(); }
        }

        public override object BuildInstance(IConfiguredInstance instance, StructureMap.Pipeline.IBuildSession session)
        {
            BasicGridColumn column = new BasicGridColumn(instance.GetProperty("headerText"));

//			column.Widget = 
//				(IWidget) Memento.GetChild("Widget", "StructureMap.Testing.Widget.IWidget", this.Manager);
//
//			column.FontStyle = 
//				(FontStyleEnum) Enum.Parse( typeof ( FontStyleEnum ), Memento.GetProperty( "FontStyle" ), true );

//			column.ColumnName = Memento.GetProperty("ColumnName");

            column.Rules =
                (Rule[])
                session.CreateInstanceArray(typeof(Rule), instance.GetChildrenArray("Rules"));

//
//			column.WrapLines = bool.Parse(Memento.GetProperty("WrapLines"));

            return column;
        }
    }
}