using System;
using StructureMap.Diagnostics;

namespace StructureMapDoctor
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args[0] == "?")
            {
                writeUsage();
                return 0;
            }

            if (args.Length == 0)
            {
                writeUsage();
                return 1;
            }


            var doctor = new Doctor
            {
                BinaryPath = AppDomain.CurrentDomain.BaseDirectory,
                BootstrapperType = args[0],
                ConfigFile = string.Empty,
                OutputFile = string.Empty
            };


            for (int i = 1; i < args.Length; i++)
            {
                string argument = args[i];
                string[] parts = argument.Split('=');
                if (parts.Length != 2)
                {
                    writeUsage();
                    return 1;
                }

                parseArgument(parts, doctor);
            }

            Console.WriteLine();
            DoctorReport report = doctor.RunReport();
            if (report.Result == DoctorResult.Success)
            {
                Console.WriteLine("SUCCESS!");
            }
            else
            {
                Console.WriteLine("FAILURE!");
            }


            return report.Result == DoctorResult.Success ? 0 : 1;
        }

        private static void parseArgument(string[] parts, Doctor doctor)
        {
            switch (parts[0])
            {
                case "ConfigFile":
                    doctor.ConfigFile = parts[1];
                    break;

                case "BinaryPath":
                    doctor.BinaryPath = parts[1];
                    break;

                case "OutputFile":
                    doctor.OutputFile = parts[1];
                    break;
            }
        }

        private static void writeUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine(
                "StructureMapDoctor.exe BOOTSTRAPPER_TYPE [ConfigFile=FILENAME] [BinaryPath=DIRECTORY] [OutputFile=FILENAME]");
            Console.WriteLine(
                "BOOTSTRAPPER_TYPE    -- Assembly qualified name of a class implementing the StructureMap.IBootstrapper interface");
            Console.WriteLine(
                "ConfigFile=FILENAME  -- Specifies the application config file if StructureMap is pulling values from the AppSettings");
            Console.WriteLine(
                "BinaryPath=DIRECTORY -- Specifies the directory containing the application assemblies.  If not specified, StructureMapDoctor");
            Console.WriteLine("                        will use the current directory");
            Console.WriteLine("OutputFile=FILENAME  -- The filename to direct any error messages and the report");
        }
    }
}