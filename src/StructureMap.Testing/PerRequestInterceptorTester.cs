using NUnit.Framework;

namespace StructureMap.Testing
{
    [TestFixture]
    public class PerRequestInterceptorTester
    {
        //Think of this as a data session
        public class Session
        {
            private static int count = 1;
            private readonly int _number = count++;

            public override string ToString()
            {
                return string.Format("Number: {0}", _number);
            }
        }

        public class Model1
        {
            public Model1(Session session)
            {
                Session = session;
            }

            public Session Session { get; set; }
        }

        public class Model2
        {
            public Model2(Session session)
            {
                Session = session;
            }

            public Session Session { get; set; }
        }

        //this is my shell, it needs some models
        public class Shell
        {
            public Shell(Model1 model1, Model2 model2)
            {
                Model1 = model1;
                Model2 = model2;
            }

            public Model1 Model1 { get; set; }
            public Model2 Model2 { get; set; }
        }

        [Test]
        public void TestObjectReturnedAreUnique()
        {
            ObjectFactory.Initialize(
                x => {
                    x.For<Session>()
                        .AlwaysUnique()
                        .Use<Session>();

                    x.For<Model1>()
                        .Use<Model1>();

                    x.For<Model2>()
                        .Use<Model2>();

                    x.For<Shell>()
                        .Use<Shell>();
                });

            var shell = ObjectFactory.GetInstance<Shell>();

            shell.Model1.Session.ShouldNotBeTheSameAs(shell.Model2.Session);
        }
    }
}