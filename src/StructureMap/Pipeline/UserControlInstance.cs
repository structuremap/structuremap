using System;
using System.Web.UI;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class UserControlInstance : ExpressedInstance<UserControlInstance>
    {
        private readonly string _url;

        public UserControlInstance(string url)
        {
            _url = url;
        }

        protected override UserControlInstance thisInstance { get { return this; } }


        public string Url { get { return _url; } }

        protected override object build(Type pluginType, BuildSession session)
        {
            Control control = new Page().LoadControl(_url);

            Type TPluggedType = control.GetType();
            if (!TPluggedType.CanBeCastTo(pluginType))
            {
                throw new StructureMapException(303, pluginType, TPluggedType);
            }

            return control;
        }

        protected override string getDescription()
        {
            return "UserControl at " + _url;
        }
    }
}