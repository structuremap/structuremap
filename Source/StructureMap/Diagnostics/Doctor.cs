using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StructureMap.Diagnostics
{
    public class Doctor
    {
        public string ConfigFile { get; set; }
        public string BootstrapperType { get; set; }
        public string BinaryPath { get; set; }
        public string OutputFile { get; set; }

        public Doctor()
        {
            BinaryPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public DoctorReport RunReport()
        {
            AppDomain domain = null;

            try
            {
                var setup = new AppDomainSetup() { ApplicationBase = BinaryPath, ConfigurationFile = ConfigFile };
                if (BinaryPath != null) setup.PrivateBinPath = BinaryPath;
                domain = AppDomain.CreateDomain("StructureMap-Diagnostics", null, setup);
                var doctor = (DoctorRunner)domain.CreateInstanceAndUnwrap(typeof(DoctorRunner).Assembly.FullName, typeof(DoctorRunner).FullName);

                DoctorReport report = doctor.RunReport(BootstrapperType);
                writeReport(report);
                writeResults(System.Console.Out, report);

                return report;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        private void writeReport(DoctorReport report)
        {
            if (string.IsNullOrEmpty(OutputFile))
            {
                return;
            }

            using (StreamWriter writer = new StreamWriter(OutputFile))
            {
                writeResults(writer, report);
            }
        }

        private void writeResults(TextWriter writer, DoctorReport report)
        {
            writer.WriteLine("StructureMap Configuration Report written at " + DateTime.Now.ToString());
            writer.WriteLine("Result:  " + report.Result.ToString());
            writer.WriteLine();
            writer.WriteLine("BootStrapper:  " + BootstrapperType);
            writer.WriteLine("ConfigFile:  " + ConfigFile);
            writer.WriteLine("BinaryPath:  " + BinaryPath);
            writer.WriteLine("====================================================================================================");
            
            writer.WriteLine();
            writer.WriteLine();

            if (!string.IsNullOrEmpty(report.ErrorMessages))
            {
                writer.WriteLine("====================================================================================================");
                writer.WriteLine("=                                     Error Messages                                               =");
                writer.WriteLine("====================================================================================================");
                writer.WriteLine(report.ErrorMessages);
                writer.WriteLine();
                writer.WriteLine();
            }

            if (!string.IsNullOrEmpty(report.WhatDoIHave)) 
            {
                writer.WriteLine(report.WhatDoIHave);
                writer.WriteLine();
                writer.WriteLine();
            }
        }
    }
}
