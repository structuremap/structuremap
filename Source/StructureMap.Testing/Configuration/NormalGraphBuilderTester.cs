using System;
using NMock;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Configuration
{
	[TestFixture]
	public class NormalGraphBuilderTester
	{
		[Test]
		public void ScopeIsUsedToCreateTheInterceptionChain()
		{
			InstanceScope theScope = InstanceScope.PerRequest;
			InterceptionChain chain = new InterceptionChain();
			DynamicMock builderMock = new DynamicMock(typeof(IInterceptorChainBuilder));
			builderMock.ExpectAndReturn("Build", chain, theScope);

			NormalGraphBuilder graphBuilder = new NormalGraphBuilder(new InstanceDefaultManager(), (IInterceptorChainBuilder) builderMock.MockInstance);

			TypePath typePath = new TypePath(this.GetType());

			
			graphBuilder.AddPluginFamily(typePath, "something", new string[0], theScope);

			PluginFamily family = graphBuilder.PluginGraph.PluginFamilies[this.GetType()];

			Assert.AreSame(chain, family.InterceptionChain);

			builderMock.Verify();
		}
	}
}
