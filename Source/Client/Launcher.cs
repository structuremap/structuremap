using System;
using System.Windows.Forms;
using StructureMap.UserInterface;

namespace Client
{
	internal class Launcher
	{
		[STAThread]
		private static void Main( string[] args )
		{
			string path = args[0];
			ApplicationShell instance = ApplicationShell.Instance;
			instance.LoadConfigFile( path );
			Application.Run( instance );
		}
	}
}