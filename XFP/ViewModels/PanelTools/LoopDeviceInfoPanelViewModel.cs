using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls.ViewModels;
using CTecDevices.Protocol;

namespace Xfp.ViewModels.PanelTools
{
    /// <summary>
    /// Viewmodel for LoopDeviceInfoPanelView.xaml
    /// </summary>
    public class LoopDeviceInfoPanelViewModel : ViewModelBase
    {
        public LoopDeviceInfoPanelViewModel()
        {
        }


        private string _siteName;
        private int    _deviceNumber;
        private int    _deviceType;
        private string _deviceName;
        private string _zone;
        private int    _sensitivityDay;
        private int    _sensitivityNight;
        private bool   _remoteLed;
        private bool   _hasAncillaryBaseSounder;
        private bool   _hasAddressableBaseSounder;

        public string SiteName         { get => _siteName;         set { _siteName = value; OnPropertyChanged(); } }
        public int    DeviceNumber     { get => _deviceNumber;     set { _deviceNumber = value; OnPropertyChanged(); } }
        public int    DeviceType       { get => _deviceType;       set { _deviceType = value; OnPropertyChanged(); } }
        public string DeviceName       { get => _deviceName;       set { _deviceName = value; OnPropertyChanged(); } }
        public string Zone             { get => _zone;             set { _zone = value; OnPropertyChanged(); } }
        public int    SensitivityDay   { get => _sensitivityDay;   set { _sensitivityDay = value; OnPropertyChanged(); } }
        public int    SensitivityNight { get => _sensitivityNight; set { _sensitivityNight = value; OnPropertyChanged(); } }
        public bool   RemoteLedEnabled { get => _remoteLed;        set { _remoteLed = value; OnPropertyChanged(); } }
        public bool   HasAncillaryBaseSounder   { get => _hasAncillaryBaseSounder;   set { _hasAncillaryBaseSounder = value; OnPropertyChanged(); } }
        public bool   HasAddressableBaseSounder { get => _hasAddressableBaseSounder; set { _hasAddressableBaseSounder = value; OnPropertyChanged(); } }
        
        public Visibility RemoteLedVisibility              { get => DeviceTypes.CurrentProtocolIsXfpApollo ? Visibility.Visible : Visibility.Collapsed; }
        public Visibility AncillaryBaseSounderVisibility   { get => DeviceTypes.CurrentProtocolIsXfpApollo ? Visibility.Visible : Visibility.Collapsed; }
    }
}
