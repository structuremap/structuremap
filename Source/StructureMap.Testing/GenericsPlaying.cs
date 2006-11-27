using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mettle.Gauntlet.FieldAgent.UI.Data;
using Mettle.Gauntlet.FieldAgent.UI.Interfaces;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class GenericsPlaying
    {
        [SetUp]
        public void SetUp()
        {
        }


        [Test, Explicit]
        public void Playing()
        {
            Type type = typeof (IGenericSomething<int>);
            Debug.WriteLine(type.AssemblyQualifiedName);
            
            PluginFamily family = new PluginFamily(type);
        }


        [Test, Explicit]
        public void JeffreyGilbertProblem()
        {
            //PluginGraph graph = new PluginGraph();
            //graph.Assemblies.Add(typeof (IDataFetcher<IUnit> ).Assembly);
            //graph.Seal();
            //InstanceManager manager = new InstanceManager(graph);

            //IDataUpdateCalculator<IUnit> child1 = (IDataUpdateCalculator<IUnit>) manager.CreateInstance(typeof(IDataUpdateCalculator<IUnit>));
            ObjectFactory.GetInstance<IDataUpdateCalculator<IUnit>>();
            
            //IDataUpdateCoordinator<IUnit> instance = (IDataUpdateCoordinator<IUnit>) manager.CreateInstance(typeof (IDataUpdateCoordinator<IUnit>));
            //Assert.IsNotNull(instance);
        }

    }
    
    
    
    public interface IGenericSomething<T>
    {
        void DoSomething(T thing);
    }
}
