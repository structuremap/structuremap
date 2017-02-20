using System;
using Shouldly;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_519_Uri_as_explicit_arg
    {
        [Fact]
        public void restclient_uri_should_not_be_null()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<RestClientTest>()
                 .Configure
                 .Ctor<string>("userAgent")
                 .Is(() => "UA")
                 .Ctor<Uri>("uri")
                 .Is(() => null);
            });


            var rc = container.With("uri").EqualTo(new Uri("http://www.google.com")).GetInstance<RestClientTest>();

            rc.Uri.ShouldBe(new Uri("http://www.google.com"));
        }

        [Fact]
        public void restclient_uris_should_be_different()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<RestClientTest>()
                 .Configure
                 .Ctor<string>("userAgent")
                 .Is(() => "UA")
                 .Ctor<Uri>("uri")
                 .Is(() => null);
            });


            var rc1 = container.With("uri").EqualTo(new Uri("http://www.google.com")).GetInstance<RestClientTest>();
            var rc2 = container.With("uri").EqualTo(new Uri("http://www.amazon.com")).GetInstance<RestClientTest>();

            Assert.True(rc1.Uri.AbsoluteUri != rc2.Uri.AbsoluteUri);
        }

        public class RestClientTest
        {
            public string UserAgent { get; set; }
            public Uri Uri { get; set; }

            public RestClientTest(string userAgent, Uri uri)
            {
                UserAgent = userAgent;
                Uri = uri;
            }
        }
    }
}