using System.Reflection;
using StructureMap.Client.Controllers;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [Pluggable("Basic")]
    public class BasicView : IHTMLSource
    {
        private readonly string _headerText;
        private readonly string _subjectMember;
        private readonly IViewPart[] _parts;

        public BasicView(string headerText, string subjectMember, IViewPart[] parts)
        {
            _headerText = headerText;
            _subjectMember = subjectMember;
            _parts = parts;
        }

        public string GetHeaderText(GraphObject subject)
        {
            if (_subjectMember == string.Empty)
            {
                return _headerText;
            }
            else
            {
                PropertyInfo property = subject.GetType().GetProperty(_subjectMember);
                string headerSuffix = property.GetValue(subject, null).ToString();

                return string.Format("{0}:  {1}", _headerText, headerSuffix);
            }
        }

        public string BuildHTML(GraphObject subject)
        {
            HTMLBuilder builder = new HTMLBuilder();

            string headerDisplay = GetHeaderText(subject);
            builder.AddHeader(headerDisplay);

            foreach (IViewPart part in _parts)
            {
                part.WriteHTML(builder, subject);
            }

            return builder.HTML;
        }
    }
}