using System;
using System.Web.UI;

namespace StructureMap.Pipeline
{
    public class UserControlInstance : ExpressedInstance<UserControlInstance>
    {
        private readonly string _url;

        public UserControlInstance(string url)
        {
            _url = url;
        }

        protected override UserControlInstance thisInstance
        {
            get { return this; }
        }


        public string Url
        {
            get { return _url; }
        }

        protected override object build(Type pluginType, IBuildSession session)
        {
            // TODO:  VALIDATE that the type works
            return new Page().LoadControl(_url);
        }
    }
}