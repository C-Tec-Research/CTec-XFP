using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Interfaces
{
    interface IXfpToolsPanelConfigPage : IXfpToolsPage
    {
        //void SetDataContext(XfpFileViewModel context);
        void SetPanel(int panel);
    }
}
