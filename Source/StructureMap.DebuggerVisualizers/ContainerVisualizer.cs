using System;
using System.Diagnostics;
using Microsoft.VisualStudio.DebuggerVisualizers;
using StructureMap;
using StructureMap.DebuggerVisualizers;

[assembly: DebuggerVisualizer(typeof(ContainerVisualizer), typeof(ContainerVisualizerObjectSource), Target = typeof(Container), Description = "Container Browser")]

namespace StructureMap.DebuggerVisualizers
{

    public class ContainerVisualizer : DialogDebuggerVisualizer
    {
        private IDialogVisualizerService modalService;

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            modalService = windowService;
            if (modalService == null) throw new NotSupportedException("This debugger does not support modal visualizers");
            
            var containerDetail = (ContainerDetail)objectProvider.GetObject();
            var form = new ContainerForm(containerDetail);
            modalService.ShowDialog(form);
        }
    }
}
