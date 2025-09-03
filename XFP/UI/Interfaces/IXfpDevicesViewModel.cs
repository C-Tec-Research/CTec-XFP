using static CTecControls.Config.DeviceSelectorConfig;

namespace Xfp.UI.Interfaces
{
    public interface IXfpDevicesViewModel
    {
        void InitDeviceSelector();

        public delegate void MenuInitialiser(DeviceSelectorMenu menuConfig);
    }
}
