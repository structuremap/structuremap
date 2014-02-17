
using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.StructureMap3;
using HtmlTags;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Web;

namespace AspNetHarness
{
    public class AspNetRegistry : Registry
    {
        public AspNetRegistry()
        {
            For<HttpContextTracked>().HttpContextScoped().Use<HttpContextTracked>();
        }
    }

    public class AspNetApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.DefaultPolicies().StructureMap<AspNetRegistry>();
        }
    }

    public class HttpContextTracked : IDisposable
    {
        public readonly Guid Name = Guid.NewGuid();
        public void Dispose()
        {
            DisposeTracker.DisposedThings.Add(this);
        }
    }

    public static class DisposeTracker
    {
        public static readonly List<HttpContextTracked> DisposedThings = new List<HttpContextTracked>(); 
    }

    public class HomeEndpoint
    {
        private readonly IContainer _container;

        public HomeEndpoint(IContainer container)
        {
            _container = container;
        }

        public HtmlDocument Index()
        {
            var doc = new HtmlDocument {Title = "Asp.Net HttpContext"};

            doc.Add("h1")
                .Text(
                    "The list below is the result of requesting an HttpContext lifecycled object from the container 5 times.  You should see the same Guid all 5 times.  If you refresh, you should see a different Guid");


            doc.Push("ul");
            doc.Add("li").Text(_container.GetInstance<HttpContextTracked>().Name.ToString());
            doc.Add("li").Text(_container.GetInstance<HttpContextTracked>().Name.ToString());
            doc.Add("li").Text(_container.GetInstance<HttpContextTracked>().Name.ToString());
            doc.Add("li").Text(_container.GetInstance<HttpContextTracked>().Name.ToString());
            doc.Add("li").Text(_container.GetInstance<HttpContextTracked>().Name.ToString());


            doc.Pop();


            doc.Add("hr");
            doc.Add("p").Text("The following is a list of previously disposed HttpContextTraced objects");
            doc.Push("ul");

            DisposeTracker.DisposedThings.Each(x => doc.Add("li").Text(x.Name.ToString()));


            return doc;
        }
    }
}