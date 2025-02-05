using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CTecControls;
using CTecControls.ViewModels;
using CTecDevices.Protocol;

namespace Xfp.ViewModels
{
    /// <summary>
    /// Viewmodel for Device.xaml
    /// </summary>
    public class DeviceViewModel : ViewModelBase
    {
        public DeviceViewModel() { }


        private int _deviceType;
        private BitmapImage _deviceIcon;
        private int _deviceIndex;
        private string _deviceTypeName;
        private int _zone;
        private string _name;
        private string _hint;
        private int _group;
        private int _nameIndex;
        private bool _hasBaseSounder;
        private bool _typeChanged;
        private ObservableCollection<int> _data;

        public int DeviceType
        {
            get => _deviceType;
            set
            {
                _deviceType = value;
                OnPropertyChanged();
                DeviceIcon = DeviceTypes.DeviceIcon(DeviceType, DeviceTypes.CurrentProtocolType);
                DeviceTypeName = DeviceTypes.DeviceTypeName(DeviceType, DeviceTypes.CurrentProtocolType);
            }            
        }
        public BitmapImage DeviceIcon { get => _deviceIcon; private set { _deviceIcon = value; OnPropertyChanged(); } }
        public int DeviceIndex { get => _deviceIndex; set { _deviceIndex = value; OnPropertyChanged(); } }
        public string DeviceTypeName { get =>_deviceTypeName; private set { _deviceTypeName = value; OnPropertyChanged(); } }
        public int Zone { get => _zone; set { _zone = value; OnPropertyChanged(); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Hint { get => _hint; set { _hint = value; OnPropertyChanged(); } }
        public int Group { get => _group; set { _group = value; OnPropertyChanged(); } }
        public int NameIndex { get => _nameIndex; set { _nameIndex = value; OnPropertyChanged(); } }
        public bool HasBaseSounder { get => _hasBaseSounder; set { _hasBaseSounder = value; OnPropertyChanged(); } }
        public bool TypeChanged { get => _typeChanged; set { _typeChanged = value; OnPropertyChanged(); } }
        public ObservableCollection<int> Data { get => _data; set { _data = value; OnPropertyChanged(); } }
    }


    //public class ListInt : ViewModelBase
    //{
    //    public ListInt() { }
    //    private int _value;
    //    public int Value { get => _value; set { _value = value; OnPropertyChanged(); } }
    //}


    //public class ListString : ViewModelBase
    //{
    //    public ListString() { }
    //    private string _value;
    //    public string Value { get => _value; set { _value = value; OnPropertyChanged(); } }
    //}
}
