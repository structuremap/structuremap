using System;
using System.Web.UI;

namespace StructureMap.Pipeline
{
    public class UserControlInstance : Instance
    {
        private readonly string _url;

        public UserControlInstance(string url)
        {
            _url = url;
        }


        protected override object build(Type type, IInstanceCreator creator)
        {
            // TODO:  VALIDATE that the type works
            return new Page().LoadControl(_url);
        }
    }
}