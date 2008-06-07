using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Graph;

namespace StructureMap.Diagnostics
{


    public class DoctorReport
    {
        public string Message;
        public bool Failure;
    }

    public class DoctorRunner : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public DoctorReport RunReport(string bootstrapperTypeName)
        {
            // TODO:  bootstrapperType cannot be found
            // TODO:  bootstrapperType blows up
            TypePath path = new TypePath(bootstrapperTypeName);
            Type bootstrapperType = path.FindType();

            IBootstrapper bootstrapper = (IBootstrapper) Activator.CreateInstance(bootstrapperType);

            // TODO - random error
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            // TODO -- Fails on constructor
            if (graph.Log.ErrorCount > 0)
            {
                
            }


        }
    }
    
    public class Doctor
    {
    }
}
