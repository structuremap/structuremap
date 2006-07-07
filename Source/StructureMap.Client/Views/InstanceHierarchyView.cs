using System;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Client.Views
{
	[Pluggable("InstanceHierarchy")]
	public class InstanceHierarchyView : IViewPart, IConfigurationVisitor
	{
		private XmlElement _table;
		private int _level = -1;
		private CellMaker _cell;

		public InstanceHierarchyView()
		{
		}

		public void WriteHTML(HTMLBuilder builder, GraphObject subject)
		{
			builder.AddDivider();
			_table = builder.AddTable();
			GraphObjectIterator iterator = new GraphObjectIterator(this);
			iterator.Visit(subject);
		}


		public void StartObject(GraphObject node)
		{
			_level++;



			XmlElement row = _table.OwnerDocument.CreateElement("tr");
			XmlElement td = _table.OwnerDocument.CreateElement("td");
			_table.AppendChild(row);
			row.AppendChild(td);

			_cell = new CellMaker(td);
			_cell.Level = _level;
		}

		public void EndObject(GraphObject node)
		{
			if (node.Problems.Length > 0)
			{
				_cell.MarkProblems(node);
			}

			if (node is InstanceToken)
			{
				_cell.AddText("<br></br>");
			}

			_level--;
		}

		public void HandleAssembly(AssemblyToken assembly)
		{
			throw new NotImplementedException();
		}

		public void HandleFamily(FamilyToken family)
		{
			throw new NotImplementedException();
		}

		public void HandleMementoSource(MementoSourceInstanceToken source)
		{
			HandleInstance(source);
		}

		public void HandlePlugin(PluginToken plugin)
		{
			throw new NotImplementedException();
		}

		public void HandleInterceptor(InterceptorInstanceToken interceptor)
		{
			HandleInstance(interceptor);
		}


		public void HandleInstance(InstanceToken instance)
		{
			_cell.AddText("<hr></hr>");
			_cell.AddText("Instance:  Type = ");

			string pluginLink = string.Format("http://PluginType={0}:ConcreteKey={1}", instance.PluginTypeName, instance.ConcreteKey);
			_cell.AddLink(instance.ConcreteKey, pluginLink);
			
			_cell.AddText(", Configuration Source = ");
			_cell.AddText(instance.Source.ToString());
		}

		public void HandlePrimitiveProperty(PrimitiveProperty property)
		{
			string text = string.Format("Property:  <b>{0}</b> ({1}) = {2}", property.PropertyName, property.PropertyType, property.Value);
			_cell.AddText(text);
		}

		public void HandleEnumerationProperty(EnumerationProperty property)
		{
			string text = string.Format("Property:  <b>{0}</b> ({1}) = {2}", property.PropertyName, property.PropertyType, property.Value);
			_cell.AddText(text);
		}

		public void HandleInlineChildProperty(ChildProperty property)
		{
			startChildProperty(property);
			_cell.AddText(") = Inline Definition");
		}

		private void startChildProperty(ChildProperty property)
		{
			string preamble = string.Format("Property:  <b>{0}</b> (", property.PropertyName);
			_cell.AddText(preamble);
	
			string familyLink = "PluginType=" + property.PluginTypeName;
			_cell.AddLink(property.PluginTypeName, familyLink);
		}

		public void HandleDefaultChildProperty(ChildProperty property)
		{
			startChildProperty(property);
			_cell.AddText(") = ");

			string defaultUrl = string.Format("PluginType={0}:DefaultInstance", property.PluginTypeName);
			_cell.AddLink("Default Instance", defaultUrl);
		}

		public void HandleNotDefinedChildProperty(ChildProperty property)
		{
			startChildProperty(property);
			_cell.AddText(") <b>Not Defined!</b>");
		}

		public void HandleTemplate(TemplateToken template)
		{
			throw new NotImplementedException();
		}

		public void HandleTemplateProperty(TemplateProperty property)
		{
			string text = string.Format("Template Property:  <b>{0}</b> ({1}) = {2}", property.PropertyName, property.PropertyType, property.PropertyValue);
			_cell.AddText(text);
		}

		public void HandleReferenceChildProperty(ChildProperty property)
		{
			startChildProperty(property);
			_cell.AddText(") References ") ;

			string defaultUrl = string.Format("http://PluginType={0}:InstanceKey={1}", property.PluginTypeName, property.ReferenceKey);
			_cell.AddLink(property.ReferenceKey, defaultUrl);
		}

		public void HandlePropertyDefinition(PropertyDefinition propertyDefinition)
		{
			throw new NotImplementedException();
		}

		public void HandleChildArrayProperty(ChildArrayProperty property)
		{
			string preamble = string.Format("Property:  <b>{0}</b> (Array of ", property.PropertyName);
			_cell.AddText(preamble);

			string familyLink = "PluginType=" + property.PropertyType;
			_cell.AddLink(property.PropertyType, familyLink);
			_cell.AddText(")");
		}


	}
}
