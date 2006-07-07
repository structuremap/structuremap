using System;
using System.IO;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Verification;

namespace StructureMap.Diagnostics
{
	internal class Doctor
	{
		[STAThread]
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				writeHelp();
				return;
			}


			string filePath = args[0];
			if (!File.Exists(filePath))
			{
				Console.WriteLine("Cannot find file " + filePath);
				writeHelp();
				return;
			}


			RemoteGraphContainer container = new RemoteGraphContainer(filePath);
			RemoteGraph graph = container.GetRemoteGraph();
			PluginGraphReport report = graph.GetReport();
			PluginGraphConsoleWriter consoleWriter = new PluginGraphConsoleWriter(report);

			for (int i = 1; i < args.Length; i++)
			{
				setArg(consoleWriter, args[i]);
			}

			consoleWriter.Write(Console.Out);
		}

		private static void setArg(PluginGraphConsoleWriter consoleWriter, string arg)
		{
			switch (arg)
			{
				case "-All":
					consoleWriter.WriteAll = true;
					break;

				case "-Plugins":
					consoleWriter.IncludePlugins = true;
					break;

				case "-Instances":
					consoleWriter.IncludeAllInstances = true;
					break;

				case "-Source":
					consoleWriter.IncludeSource = true;
					break;

				case "-Problems":
					consoleWriter.WriteProblems = true;
					break;

				default:
					writeHelp();
					throw new ApplicationException("Invalid Input");
			}

		}


		private static void writeHelp()
		{
			string usage = "StructureMapDoctor <Config File> [-All] [-Plugins] [-Instances] [-Source]";
			Console.WriteLine(usage);
			Console.WriteLine("");

			writeAttributeHelp("Config File", "The path to the StructureMap config file");
			writeAttributeHelp("-All", "List all objects and types");
			writeAttributeHelp("-Plugins", "List Plugin's for each PluginFamily");
			writeAttributeHelp("-Instances", "List configured instances for each PluginFamily");
			writeAttributeHelp("-Source", "List the MementoSource for each PluginFamily");
			writeAttributeHelp("-Problems", "List any problems with the Configuration");
		}

		private static void writeAttributeHelp(string flag, string description)
		{
			flag = flag.PadRight(30);

			Console.WriteLine(flag + description);
		}
	}
}