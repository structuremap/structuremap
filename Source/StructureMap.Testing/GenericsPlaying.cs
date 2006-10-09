using System;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Graph;

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
            Debug.WriteLine(type.FullName);
            
            PluginFamily family = new PluginFamily(type);
        }
    }
    
    
    public interface IGenericSomething<T>
    {
        void DoSomething(T thing);
    }
}
