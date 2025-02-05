using CTecControls.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CTecControls.Config.DeviceSelectorConfig;

namespace Xfp.UI.Interfaces
{
    public interface IXfpDevicesViewModel
    {
        void InitDeviceSelector();

        public delegate void MenuInitialiser(DeviceSelectorMenu menuConfig);
    }
}
