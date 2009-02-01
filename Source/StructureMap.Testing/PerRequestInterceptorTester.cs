using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    [TestFixture]
    public class PerRequestInterceptorTester
    {
        //Think of this as a data session
        public class Session
        {
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
                x =>
                {
                    x.BuildInstancesOf<Session>()
                        .AlwaysUnique()
                        .TheDefaultIsConcreteType<Session>();
                    x.BuildInstancesOf<Model1>()
                        .TheDefaultIsConcreteType<Model1>();
                    x.BuildInstancesOf<Model2>()
                        .TheDefaultIsConcreteType<Model2>();
                    x.BuildInstancesOf<Shell>()
                        .TheDefaultIsConcreteType<Shell>();
                });

            var shell = ObjectFactory.GetInstance<Shell>();

            shell.Model1.Session.ShouldNotBeTheSameAs(shell.Model2.Session);

        }
    }
}