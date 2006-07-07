using System.Windows.Forms;
using StructureMap.Client.Shell;

namespace StructureMapExplorer
{
	public class Runner
	{
		public Runner()
		{
		}

		public static void Main(string[] args)
		{
			ApplicationShell shell = new ApplicationShell();
			Application.Run(shell);
		}
	}
}
