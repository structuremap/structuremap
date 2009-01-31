using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;

namespace StructureMap.Prism
{
    public abstract class PrismBootstrapper<THIS> : IBootstrapper where THIS : IBootstrapper, new()
    {
        private static bool _hasStarted = false;

        public static void Restart()
        {
            
        }

        public void Bootstrap(string profile)
        {
            Bootstrap();
        }

        public void Bootstrap()
        {
            
        }

        public PrismBootstrapper(string defaultProfofile)
        {
        }

        public PrismBootstrapper() : this(string.Empty)
        {
        }


        public void BootstrapStructureMap()
        {
            throw new System.NotImplementedException();
        }

        // TODO
        // 1.) Calls all Startables at end
        // 2.) applies Profile
        // 3.) adds the basic types
        // 4.) create the shell
        // 5.) tie into debuggers
    }

    // test this

    public interface IStartable
    {
        void Execute(IContainer container);
    }

    public class PrismRegistry : Registry
    {
        // TODO
        // 1.) OnStartUp method
        // 2.) 
    }
}
