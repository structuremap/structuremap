using System.Windows.Forms;
using StructureMap.Client.Shell;

namespace StructureMapExplorer
{
	public class Runner
	{
		public Runner()
		{
		}

        [System.STAThread]
		public static void Main(string[] args)
		{
			ApplicationShell shell = new ApplicationShell();
            Application.EnableVisualStyles();
			Application.Run(shell);
		}
	}
}
