using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture, Explicit]
    public class Debugging
    {
        [Test]
        public void look_at_expression()
        {
            var gateway = new DefaultGateway();
            Expression<Func<IContext, TopLevel>> expression =
                c => new TopLevel(new Leaf(gateway, new ColorService("red"))) {Foo = new Foo(Guid.NewGuid())};
            //Expression<Func<IContext, TopLevel>> expression = c => new TopLevel(new Leaf(gateway, new ColorService("red")));

            Debug.WriteLine(expression);
        }

        [Test]
        public void simple_expression()
        {
            Expression<Func<IContext, ColorRule>> expression = c => new ColorRule("red")
            {
                Name = "Jeremy"
            };

            //Expression<Func<IContext, ColorRule>> expression = c => new ColorRule("red");

            Debug.WriteLine(expression);
        }
    }


    public static class goer
    {
        public static void go()
        {
            Func<IContext, IGateway> gatewayBuilder = c => new FakeGateway();
            var gatewayBuilder2 = DeepException.Wrap(gatewayBuilder, "new FakeGateway()");

            Func<IContext, Leaf> leafBuilder = c => new Leaf(gatewayBuilder2(c), new ColorService("red"));
            var leafBuilder2 = DeepException.Wrap(leafBuilder, "new Leaf(IGateway, IService)");

            Func<IContext, TopLevel> topLevelBuilder = context => new TopLevel(leafBuilder2(context));
            var func = DeepException.Wrap(topLevelBuilder, "new TopLevel(Leaf)");


            func(new BuildSession(new RootPipelineGraph(new PluginGraph())));
        }
    }

    public class UsesGateways
    {
        public UsesGateways(List<IGateway> gateways)
        {
        }
    }


    public class DeepException : Exception
    {
        public static Func<IContext, T> Wrap<T>(Func<IContext, T> inner, string description)
        {
            return context => {
                try
                {
                    return inner(context);
                }
                catch (DeepException e)
                {
                    e.Push(description);
                    throw;
                }
                catch (Exception e)
                {
                    throw new DeepException(description, e);
                }
            };
        }

        private void Push(string description)
        {
            _descriptions.Enqueue(description);
        }

        private readonly Queue<string> _descriptions = new Queue<string>();

        public DeepException(string message, Exception innerException) : base(null, innerException)
        {
            _descriptions.Enqueue(message);
        }

        public override string Message
        {
            get
            {
                var writer = new StringWriter();
                writer.WriteLine();
                writer.WriteLine("StructureMap session from inner to outer:");
                var i = 0;
                _descriptions.Each(x => writer.WriteLine(++i + ".) " + x));

                return writer.ToString();
            }
        }
    }

    public class TopLevel
    {
        private readonly Leaf _leaf;

        public TopLevel(Leaf leaf)
        {
            _leaf = leaf;
        }

        public IFoo Foo { get; set; }
    }

    public class Leaf
    {
        public Leaf(IGateway gateway, IService service)
        {
        }
    }

    public class FakeGateway : IGateway
    {
        public FakeGateway()
        {
            throw new ApplicationException("Kaboom!");
        }

        public string WhoAmI { get; private set; }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}