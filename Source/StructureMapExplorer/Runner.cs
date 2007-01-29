using System;
using System.Windows.Forms;
using StructureMap.Client.Shell;

namespace StructureMapExplorer
{
    public class Runner
    {
        public Runner()
        {
        }

        [STAThread]
        public static void Main(string[] args)
        {
            ApplicationShell shell = new ApplicationShell();
            Application.EnableVisualStyles();
            Application.Run(shell);
        }
    }
}