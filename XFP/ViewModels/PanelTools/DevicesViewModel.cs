using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CTecControls.UI;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;
using CTecDevices.Protocol;
using Xfp.UI.Views.PanelTools;
using XFP.Config;
using static Xfp.DataTypes.PanelData.XfpPanelData;
using Xfp.UI.ViewHelpers;
using CTecDevices;
using CTecDevices.DeviceTypes;
using System.Windows.Xps;
using CTecUtil.StandardPanelDataTypes;
using CTecControls.UI.ViewHelpers;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using CTecUtil;
using CTecUtil.Printing;
using System.Diagnostics.Eventing.Reader;

namespace Xfp.ViewModels.PanelTools
{
    /// <summary>
    /// Viewmodel for Devices.xaml
    /// </summary>
    public class DevicesViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel, IXfpDevicesViewModel
    {
        public DevicesViewModel(FrameworkElement parent) : base(parent) { }

        public DevicesViewModel(FrameworkElement parent, DeviceInfoPanel infoPanel, int loopNum = 1) : base(parent)
        {
            _loops = new() { Loop1, Loop2 };

            if (infoPanel is not null)
            {
                _infoPanelViewModel = infoPanel.DataContext as DeviceInfoPanelViewModel;
                _infoPanelViewModel.DisplayShowFittedDevicesOnlyOption = false;
                _infoPanelViewModel.GetDeviceName = getDeviceName;
                _infoPanelViewModel.SetDeviceName = setDeviceName;
            }

            DeviceConfigData.GetDeviceName = getDeviceName;

            _loopNum = loopNum;
            DeviceNameKeyToValueConverter.GetDeviceName = getDeviceName;
        }



        protected ObservableCollection<DeviceItemViewModel> _loop1 = new();
        protected ObservableCollection<DeviceItemViewModel> _loop2 = new();
        protected List<ObservableCollection<DeviceItemViewModel>> _loops;
        public int NumLoops { get => _data.CurrentPanel.LoopConfig.NumLoops; set { _data.CurrentPanel.LoopConfig.NumLoops = value; OnPropertyChanged(); } }

        public List<ObservableCollection<DeviceItemViewModel>> Loops => new() { _loop1, _loop2 };


        public ObservableCollection<DeviceItemViewModel> Loop1       { get => _loop1; set { SetValue(ref _loop1, value, nameof(Loop1)); } }
        public ObservableCollection<DeviceItemViewModel> Loop2       { get => _loop2; set { SetValue(ref _loop2, value, nameof(Loop2)); } }
        public ObservableCollection<DeviceItemViewModel> CurrentLoop { get => LoopNum > 1 ? Loop2 : Loop1; set { if (LoopNum > 1) Loop2 = value; else Loop1 = value; } }
        public bool LoopIsFitted => LoopNum <= NumLoops;

        
        //internal DeviceNamesConfigData DeviceNames;

        public override bool DebugMode { get => base.DebugMode; set { base.DebugMode = value; if (_infoPanelViewModel is not null) _infoPanelViewModel.DebugMode = value; } }


        #region loop
        protected int _loopNum;
        
        public virtual int LoopNum
        {
            get => _loopNum;
            set { /*LoopNum is constant for the overview pages; this is overridden in DeviceDetailsViewModel*/ }
        }
        public string LoopNumberDesc { get => string.Format(Cultures.Resources.Loop_x, _loopNum); set { } }

        public bool IsLoop1 { get => _loopNum == 1; set => LoopNum = value ? 1 : 2; }


        internal delegate void LoopChangedHandler(int loop);
        internal LoopChangedHandler LoopChanged;


        protected DeviceInfoPanelViewModel _infoPanelViewModel;
        #endregion loop


        protected ObservableCollection<DeviceItemViewModel> _loop1SelectedItems = new();
        protected ObservableCollection<DeviceItemViewModel> _loop2SelectedItems = new();
        public ObservableCollection<DeviceItemViewModel> Loop1SelectedItems { get => _loop1SelectedItems; set { SetValue(ref _loop1SelectedItems, value, nameof(Loop1SelectedItem)); } }
        public ObservableCollection<DeviceItemViewModel> Loop2SelectedItems { get => _loop2SelectedItems; set { SetValue(ref _loop2SelectedItems, value, nameof(Loop2SelectedItem)); } }
        public ObservableCollection<DeviceItemViewModel> SelectedItems
        {
            get => IsLoop1 ? _loop1SelectedItems : _loop2SelectedItems;
            set
            {
                if (IsLoop1)
                    Loop1SelectedItems = value;
                else
                    Loop2SelectedItems = value;
                OnPropertyChanged();
            }
        }


        public DeviceItemViewModel Loop1SelectedItem { get => _loop1SelectedItems.Count > 0 ? _loop1SelectedItems[0] : null; set { _loop1SelectedItems = new() { value }; OnPropertyChanged(); OnPropertyChanged(nameof(Loop1SelectedItems)); } }
        public DeviceItemViewModel Loop2SelectedItem { get => _loop2SelectedItems.Count > 0 ? _loop2SelectedItems[0] : null; set { _loop2SelectedItems = new() { value }; OnPropertyChanged(); OnPropertyChanged(nameof(Loop2SelectedItems)); } }
        
        public DeviceItemViewModel SelectedItem
        {
            get => IsLoop1 ? _loop1SelectedItems[0] : _loop2SelectedItems[0];
            set
            {
                if (IsLoop1)
                    Loop1SelectedItem = value;
                else
                    Loop2SelectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedItems)); 
            }
        }


        /// <summary>
        /// Update the SelectedItems list to match the DataGrid or ListView's SelectedItems
        /// </summary>
        public void ChangeSelection(System.Collections.IList selectedItems)
        {
            if (LoopNum == 2)
            {
                _loop2SelectedItems = new();
                foreach (var item in selectedItems)
                    if (item is not null)
                        _loop2SelectedItems.Add(item as DeviceItemViewModel);

                if (_infoPanelViewModel is not null)
                    _infoPanelViewModel.DeviceList = _loop2SelectedItems;
            }
            else
            {
                _loop1SelectedItems = new();
                foreach (var item in selectedItems)
                    if (item is not null)
                        _loop1SelectedItems.Add(item as DeviceItemViewModel);

                if (_infoPanelViewModel is not null)
                    _infoPanelViewModel.DeviceList = _loop1SelectedItems;
            }
        }


        public int? DeviceSelectorDeviceType
        {
            get => _infoPanelViewModel?.DeviceSelectorDeviceType;
            set
            {
                if (_infoPanelViewModel is not null)
                    _infoPanelViewModel.DeviceSelectorDeviceType = value;
                foreach (var d in Loop1)
                    d.DeviceSelectorDeviceType = value;
                foreach (var d in Loop2)
                    d.DeviceSelectorDeviceType = value;
            }
        }


        public void ChangeDeviceType(int? deviceType) => _infoPanelViewModel?.ChangeDeviceType(deviceType);
        public void DeleteDevices() => _infoPanelViewModel?.DeleteDevices();


        public void MovePrev(ListView list) { if (movePrev() is DeviceItemViewModel z) list.ScrollIntoView(z); }
        public void MoveNext(ListView list) { if (moveNext() is DeviceItemViewModel z) list.ScrollIntoView(z); }

        protected DeviceItemViewModel movePrev()
        {
            DeviceItemViewModel newSelectedDev = null;

            if (IsLoop1)
            {
                if (Loop1SelectedItems.Count == 0)
                    return Loop1SelectedItem = Loop1[0];
                var firstIndex = Loop1.Count;
                foreach (var d in from d in Loop1SelectedItems
                                  where d.Index < firstIndex
                                  select d)
                    firstIndex = d.Index;
                newSelectedDev = Loop1[firstIndex > 0 ? firstIndex - 1 : Loop1.Count - 1];
                if (_infoPanelViewModel is not null)
                    _infoPanelViewModel.DeviceList = new() { newSelectedDev };
                SelectedItem = newSelectedDev;
            }
            else
            {
                if (Loop2SelectedItems.Count == 0)
                    return Loop2SelectedItem = Loop2[0];
                var firstIndex = Loop2.Count;
                foreach (var d in from d in Loop2SelectedItems
                                  where d.Index < firstIndex
                                  select d)
                    firstIndex = d.Index;
                newSelectedDev = Loop2[firstIndex > 0 ? firstIndex - 1 : Loop2.Count - 1];
                if (_infoPanelViewModel is not null)
                    _infoPanelViewModel.DeviceList = new() { newSelectedDev };
                SelectedItem = newSelectedDev;
            }

            RefreshView();
            return newSelectedDev;
        }
        
        protected DeviceItemViewModel moveNext() 
        {
            if (IsLoop1)
            {
                if (Loop1SelectedItems.Count == 0)
                    return Loop1SelectedItem = Loop1[0];
                var lastIndex = 0;
                foreach (var d in from d in Loop1SelectedItems
                                  where d.Index > lastIndex
                                  select d)
                    lastIndex = d.Index;
                var newSelectedDev = Loop1[lastIndex < Loop1.Count - 1 ? lastIndex + 1 : 0];
                if (_infoPanelViewModel is not null)
                    _infoPanelViewModel.DeviceList = new() { newSelectedDev };
                SelectedItem = newSelectedDev;
                RefreshView();
                return newSelectedDev;
            }
            else
            {
                if (Loop2SelectedItems.Count == 0)
                    return Loop2SelectedItem = Loop2[0];
                var lastIndex = 0;
                foreach (var d in from d in Loop2SelectedItems
                                  where d.Index > lastIndex
                                  select d)
                    lastIndex = d.Index;
                var newSelectedDev = Loop2[lastIndex < Loop2.Count - 1 ? lastIndex + 1 : 0];
                if (_infoPanelViewModel is not null)
                    _infoPanelViewModel.DeviceList = new() { newSelectedDev };
                SelectedItem = newSelectedDev;
                RefreshView();
                return newSelectedDev;
            }
        }


        private string getDeviceName(int index) => _data.CurrentPanel?.DeviceNamesConfig.GetName(index);


        private int setDeviceName(int index, string name)
        {
            //has name been deleted?
            if (string.IsNullOrEmpty(name))
            {
                //delete from names if it was unique
                if (countOccurrencesOfDeviceNameIndex(index) == 1)
                    _data.CurrentPanel.DeviceNamesConfig.Remove(index);
                return 0;
            }

            int newIdx;

            if (_data.CurrentPanel.DeviceNamesConfig.ContainsValue(name))
            {
                newIdx = _data.CurrentPanel.DeviceNamesConfig.DeviceNames.IndexOf(name);
            }
            else if (index == 0 || countOccurrencesOfDeviceNameIndex(index) > 1 && _data.CurrentPanel.DeviceNamesConfig.GetName(index) != name)
            {
                //name has changed and same NameIndex is in use by other device(s)
                return _data.CurrentPanel.DeviceNamesConfig.Add(name);
            }
            else
            {
                //name has changed
                newIdx = _data.CurrentPanel.DeviceNamesConfig.Update(index, name);
            }

            //delete old name if it's no longer in use
            if (newIdx != index && countOccurrencesOfDeviceNameIndex(index) == 1)
                _data.CurrentPanel.DeviceNamesConfig.Remove(index);

            return newIdx;
        }


        /// <summary>
        /// Verifies the validity of the device's NameIndexes; compacts the name list by removing empty names and any not referenced by a device;.
        /// </summary>
        private void normaliseDeviceNames()
        {
            if (_data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count == 0)
                _data.CurrentPanel.DeviceNamesConfig = DeviceNamesConfigData.InitialisedNew();

            //delete any device names that are not attached to a device
            for (int i = 1; i < _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count; i++)
                if (!string.IsNullOrEmpty(_data.CurrentPanel.DeviceNamesConfig.DeviceNames[i]))
                    if (countOccurrencesOfDeviceNameIndex(i) == 0)
                        _data.CurrentPanel.DeviceNamesConfig.Remove(i);

            //close any gaps in the list (null/empty names)
            int curr = 1;
            while (curr < _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count)
            {
                int gap = 1;

                if (string.IsNullOrEmpty(_data.CurrentPanel.DeviceNamesConfig.DeviceNames[curr]))
                {
                    for (int i = curr + 1; i < _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(_data.CurrentPanel.DeviceNamesConfig.DeviceNames[i]))
                            break;
                        gap++;
                    }

                    if (curr >= _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count - gap)
                        break;

                    if (gap > 0)
                    {
                        for (int i = curr; i < _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count - gap; i++)
                        {
                            if (curr < _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count - gap)
                            {
                                replaceDevicesNameIndex(i + gap, i);
                                _data.CurrentPanel.DeviceNamesConfig.DeviceNames[i] = _data.CurrentPanel.DeviceNamesConfig.DeviceNames[i + gap];
                                _data.CurrentPanel.DeviceNamesConfig.DeviceNames[i + gap] = null;
                            }


                            if (curr + gap >= _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count - 1)
                                break;

                        } 
                    }
                }

                curr ++;
            }

            //remove trailing null/empty names
            int num = _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count - 1;
            while (num > 0 && string.IsNullOrEmpty(_data.CurrentPanel.DeviceNamesConfig.DeviceNames[num]))
                _data.CurrentPanel.DeviceNamesConfig.DeviceNames.RemoveAt(num--);

            //remove any invalid NameIndexes from devices
            foreach (var l in _data.CurrentPanel.LoopConfig.Loops)
            {
                foreach (var d in l.Devices)
                {
                    if (d.NameIndex > 0 && !_data.CurrentPanel.DeviceNamesConfig.ContainsIndex(d.NameIndex))
                        d.NameIndex = 0;

                    if (d.IsIODevice)
                        foreach (var io in d.IOConfig)
                            if (io.NameIndex > 0 && io.InputOutput == IOTypes.NotUsed
                             || !_data.CurrentPanel.DeviceNamesConfig.ContainsIndex(io.NameIndex))
                                io.NameIndex = 0;
                }
            }
        }
        

        /// <summary>
        /// Returns a count of the number of times the given name index is referenced.<br/>
        /// NB: the index may be referenced in both the main name and in IOSettings' names.
        /// </summary>
        private int countOccurrencesOfDeviceNameIndex(int index)
        {
            var result = 0;

            foreach (var l in _data.CurrentPanel.LoopConfig.Loops)
            {
                foreach (var d in l.Devices)
                {
                    if (DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                    {
                        if (d.NameIndex == index)
                            result++;

                        if (d.IsIODevice)
                            for (int io = 1; io < d.IOConfig.Count; io++)
                                if (d.IOConfig[io].InputOutput != IOTypes.NotUsed && d.IOConfig[io].NameIndex == index)
                                    result++;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Returns a list of devices referencing the given name index.<br/>
        /// NB: the index may be referenced in both the main name and in IOSettings' names.
        /// </summary>
        private List<DeviceData> findDevicesWithDeviceNameIndex(int index)
            => [.. from loop in _data.CurrentPanel.LoopConfig.Loops
                   from dev in loop.Devices
                   where DeviceTypes.IsValidDeviceType(dev.DeviceType, DeviceTypes.CurrentProtocolType)
                   where dev.NameIndex == index
                      || dev.IsIODevice && (dev.IOConfig[1].InputOutput != IOTypes.NotUsed && dev.IOConfig[1].NameIndex == index 
                                         || dev.IOConfig[2].InputOutput != IOTypes.NotUsed && dev.IOConfig[2].NameIndex == index
                                         || dev.IOConfig[3].InputOutput != IOTypes.NotUsed && dev.IOConfig[3].NameIndex == index)
                   select dev];


        //private List<DeviceData> findDevicesWithSameName(string name)
        //{
        //    var result = new List<DeviceData>();

        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var idx = _data.DeviceNamesConfig.GetNameIndex(name);
        //        if (idx > 0)
        //            return findDevicesWithDeviceNameIndex(idx);
        //    }

        //    return result;
        //}


        /// <summary>
        /// Replaces the name index where it is used on any devices
        /// </summary>
        private void replaceDevicesNameIndex(int oldKey, int newKey)
        {
            foreach (var d in findDevicesWithDeviceNameIndex(oldKey))
            {
                if (d.NameIndex == oldKey)
                    d.NameIndex = newKey;

                if (d.IsIODevice)
                    foreach (var iod in d.IOConfig)
                                if (iod.NameIndex == oldKey)
                                    iod.NameIndex = newKey;
            }
        }


        #region ConfigToolsPageViewModelBase overrides
        public override void SetChangesAreAllowedChecker(ChangesAreAllowedChecker checker) => _infoPanelViewModel?.SetChangesAreAllowedChecker(CheckChangesAreAllowed = checker);


        public override bool IsReadOnly
        {
            get => base.IsReadOnly;
            set { if (_infoPanelViewModel is not null) _infoPanelViewModel.IsReadOnly = base.IsReadOnly = value; }
        }
        #endregion


        public IXfpDevicesViewModel.MenuInitialiser InitMenu;


        #region IAppViewModel implementation
        public virtual void SetCulture(CultureInfo culture)
        {
            PageHeader = string.Format(Cultures.Resources.Nav_Loop_x, LoopNum);

            //var save = SelectedItem;

            foreach (var d in Loop1)
                d.SetCulture(culture);
            foreach (var d in Loop2)
                d.SetCulture(culture);
            if (_infoPanelViewModel is not null)
                _infoPanelViewModel.SetCulture(culture);
            InitMenu?.Invoke(DeviceSelectorSettings.Menu);
            CultureChanged?.Invoke(culture);
            //SelectedItem = save;
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;
 
            Loop1 = new();
            Loop2 = new();

            if (data.CurrentPanel.LoopConfig.Loop1.Devices.Count > 0)
            {
                int j = 0;
                foreach (var d in data.CurrentPanel.LoopConfig.Loop1.Devices)
                {
                    DeviceItemViewModel newDev = new()
                    {
                        DeviceData = d,
                        GetDeviceName = getDeviceName,
                        SetDeviceName = setDeviceName,
                        ZoneData = data.CurrentPanel.ZoneConfig,
                        GroupData = data.CurrentPanel.GroupConfig,
                        Index = j++
                    };
                    Loop1.Add(newDev);
                }
            }

            if (NumLoops > 1)
            {
                if (data.CurrentPanel.LoopConfig.Loop2.Devices.Count > 0)
                {
                    int j = 0;
                    foreach (var d in data.CurrentPanel.LoopConfig.Loop2.Devices)
                    {
                        DeviceItemViewModel newDev = new()
                        {
                            DeviceData = d,
                            GetDeviceName = getDeviceName,
                            SetDeviceName = setDeviceName,
                            ZoneData = data.CurrentPanel.ZoneConfig,
                            GroupData = data.CurrentPanel.GroupConfig,
                            Index = j++
                        };
                        Loop2.Add(newDev);
                    }
                }
            }

            if (this is DeviceDetailsViewModel)
                if (LoopNum > NumLoops)
                    LoopNum = NumLoops;

            _infoPanelViewModel?.PopulateView(data);

            RefreshView();
        }

        public virtual void RefreshView()
        {
            if (_data is null)
                return;

            OnPropertyChanged(nameof(NumLoops));
            OnPropertyChanged(nameof(LoopNum));
            OnPropertyChanged(nameof(LoopIsFitted));
            OnPropertyChanged(nameof(IsLoop1));
            //OnPropertyChanged(nameof(Loop1));
            //OnPropertyChanged(nameof(Loop2));
            OnPropertyChanged(nameof(CurrentLoop));
            OnPropertyChanged(nameof(LoopNumberDesc));
            OnPropertyChanged(nameof(IsReadOnly));

            _infoPanelViewModel?.RefreshView();

            foreach (var d in CurrentLoop)
                d.RefreshView();
        }
        #endregion


        #region IConfigToolsPageViewModel implementation
        public void EnqueuePanelDownloadCommands(bool allPages)
        {
            string messy = null;

            //warn if current loop does not exist on the panel or if the number of
            //loops on the connected panel does not match what has been configured
            if (!allPages && LoopNum > LoopConfigData.DetectedLoops)
                messy = string.Format(Cultures.Resources.Comms_Download_Warn_Loop_x_Not_Present, LoopNum);
            else if (NumLoops != LoopConfigData.DetectedLoops)
                messy = NumLoops == 1 ? Cultures.Resources.Comms_Download_Warn_Loop_Count_2_Loops : Cultures.Resources.Comms_Download_Warn_Loop_Count_1_Loop;

            if (messy is not null)
            {
                var tit = allPages ? Cultures.Resources.Comms_Downloading_System : Cultures.Resources.Comms_Downloading_Page;
                CTecMessageBox.ShowOKInfo(messy, tit);
            }


            //get the device names list first
            //NB: the list MUST be cleared first
            _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Clear();
            PanelComms.DeviceNameReceived = deviceNameReceived;
            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Device_Names, null);
            for (int i = 0; i < NumLoops * DeviceConfigData.NumDevices; i++)
                PanelComms.AddCommandRequestDeviceName(i, String.Format(Cultures.Resources.Device_Name_x, i + 1));


            PanelComms.DeviceReceived = deviceReceived;
            PanelComms.BaseSounderGroupReceived = baseSounderGroupReceived;

            //get all loops
            //NB: DeviceNames list spans all loops, so getting only 1 from of a 2-loop panel can result in orphaned names which will get deleted
            for (int loop = 0; loop < LoopConfigData.DetectedLoops; loop++)
            {
                PanelComms.InitNewDownloadCommandSubqueue(string.Format(Cultures.Resources.Loop_x_Devices, loop + 1), null);
                for (int dev = 0; dev < DeviceConfigData.NumDevices; dev++)
                    PanelComms.AddCommandRequestDevice(loop, dev, String.Format(Cultures.Resources.Loop_x_Device_y, loop + 1, dev + 1));
            }

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                //get base sounder info
                PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Base_Sounder_Groups, allPages ? downloadRequestsCompleted : null);
                for (int loop = 0; loop < LoopConfigData.DetectedLoops; loop++)
                    for (int dev = 0; dev < DeviceConfigData.NumDevices; dev++)
                        PanelComms.AddCommandRequestBaseSounderGroup(loop, dev, String.Format(Cultures.Resources.Loop_x_Base_Sounder_Group_y, loop + 1, dev + 1));
            }
            
            if (!allPages)
            {
                //also get zone/panel names
                PanelComms.ZoneNameReceived = zoneNameReceived;
                PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Zone_Names, null);
                for (int zone = 0; zone < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; zone++)
                    PanelComms.AddCommandRequestZoneName(zone, String.Format(Cultures.Resources.Zone_Name_x, zone + 1));

                //...and the alarm verification counts
                PanelComms.AL3CodeReceived = al3CodeReceived;
                PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Site_Information, downloadRequestsCompleted);
                PanelComms.AddCommandRequestAL3Code(Cultures.Resources.AL3_Code);
            }
        }


        protected bool preUploadCheck(bool allPages, int loopNum)
        {
            if (!allPages && loopNum > LoopConfigData.DetectedLoops)
            {
                //warn if current loop does not exist on the panel
                var messy = string.Format(Cultures.Resources.Comms_Upload_Warn_Loop_x_Not_Present, loopNum);
                var tit   = Cultures.Resources.Comms_Uploading_Page;
                CTecMessageBox.ShowOKError(messy, tit);
                return false;
            }

            if (NumLoops > LoopConfigData.DetectedLoops)
            {
                //warn if current loop does not exist on the panel
                var messy = Cultures.Resources.Comms_Upload_Warn_Loop_Count_1_Loop;
                var tit   = allPages ? Cultures.Resources.Comms_Uploading_System : Cultures.Resources.Comms_Uploading_Page;
                CTecMessageBox.ShowOKInfo(messy, tit);
            }
            return true;
        }


        public virtual void EnqueuePanelUploadCommands(bool allPages)
        {
            if (!preUploadCheck(allPages, LoopNum))
                return;

            //blank loop data, to send if panel has more loops than the data
            List<DeviceItemViewModel> blank = null;

            if (NumLoops > LoopConfigData.DetectedLoops)
            {
                blank = new();
                for (int i = 0; i < DeviceConfigData.NumDevices; i++)
                    blank.Add(new DeviceItemViewModel() { DeviceData = (DeviceData)DeviceData.InitialisedNew(LoopNum) });
            }
            
            //send the device names
            normaliseDeviceNames();
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Device_Names, null);
            //NB: name #0 on the panel is always 'No name allocated', so always upload it explicitly
            PanelComms.AddCommandSetDeviceName(0, Cultures.Resources.No_Name_Allocated, string.Format(Cultures.Resources.Device_Name_x_Value_y, 1, Cultures.Resources.No_Name_Allocated));
            for (int i = 1; i < _data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count; i++)
            {
                var name = _data.CurrentPanel.DeviceNamesConfig.DeviceNames[i];
                PanelComms.AddCommandSetDeviceName(i, name, string.Format(Cultures.Resources.Device_Name_x_Value_y, i + 1, name));
            }
            PanelComms.AddCommandEndDeviceNameUpload(Cultures.Resources.End_Of_Device_Names);

            //send loops
            var start = allPages ? 1 : LoopNum;
            var finish = allPages ? LoopConfigData.MaxLoops : LoopNum;

            for (int loop = start; loop <= finish; loop++)
            {
                if (loop > LoopConfigData.DetectedLoops)
                    break;

                PanelComms.InitNewUploadCommandSubqueue(string.Format(Cultures.Resources.Loop_x_Devices, loop + 1), null);
                for (int device = 0; device < DeviceConfigData.NumDevices; device++)
                {

                    //var dev = loop <= NumLoops ? _data.CurrentPanel.LoopConfig.Loops[LoopNum - 1].Devices[device] : blank.Loops[LoopNum - 1].Devices[device];
                    var dev = loop <= NumLoops ? Loops[LoopNum - 1][device] : blank[device];
                    PanelComms.AddCommandSetDevice(dev.DeviceData, string.Format(Cultures.Resources.Loop_x_Device_y_Type_z, dev.LoopNum + 1, dev.Index + 1, 
                                                   DeviceTypes.DeviceTypeName(dev.DeviceType, DeviceTypes.CurrentProtocolType)??Cultures.Resources.Not_Fitted));
                }
            }

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                ////send base sounder info for both loops
                //PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Base_Sounder_Groups, null);
                //for (int loop = start; loop <= finish; loop++)
                //    for (int device = 0; device < DeviceConfigData.NumDevices; device++)
                //    {
                //        var dev = _data.CurrentPanel.LoopConfig.Loops[LoopNum - 1].Devices[device];
                //        PanelComms.AddCommandSetBaseSounderGroup(new(loop)
                //        {
                //            Index = device + DeviceConfigData.NumDevices + 1,
                //            DeviceType = (int)(dev.AncillaryBaseSounderGroup > 0 ? XfpApolloDeviceTypeIds.SounderController : XfpApolloDeviceTypeIds.Unknown),
                //            AncillaryBaseSounderGroup = dev.AncillaryBaseSounderGroup,
                //            IsRealDevice = false,
                //        },
                //        string.Format(Cultures.Resources.Loop_x_Base_Sounder_Group_y, loop + 1, device + 1));
                //    }
                
                //send base sounder info for loop(s)
                PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Base_Sounder_Groups, null);
                for (int loop = start; loop <= finish; loop++)
                    for (int device = 0; device < DeviceConfigData.NumDevices; device++)
                    {
                        var dev = loop <= NumLoops ? _data.CurrentPanel.LoopConfig.Loops[LoopNum - 1].Devices[device] : new DeviceData(loop);
                        PanelComms.AddCommandSetBaseSounderGroup(new(loop)
                        {
                            Index = device + DeviceConfigData.NumDevices + 1,
                            DeviceType = (int)(dev.AncillaryBaseSounderGroup > 0 ? XfpApolloDeviceTypeIds.SounderController : XfpApolloDeviceTypeIds.Unknown),
                            AncillaryBaseSounderGroup = dev.AncillaryBaseSounderGroup,
                            IsRealDevice = false,
                        },
                        string.Format(Cultures.Resources.Loop_x_Base_Sounder_Group_y, loop + 1, device + 1));
                    }
            }

            //also send the alarm verification counts
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Site_Information, uploadRequestsCompleted);
            PanelComms.AddCommandSetAL3Code(new() {
                                                AL3Code = _data.CurrentPanel.PanelConfig.AL3Code, 
                                                BlinkPollingLED = _data.CurrentPanel.PanelConfig.BlinkPollingLED, 
                                                DetectorDebounce = _data.CurrentPanel.PanelConfig.DetectorDebounce, 
                                                IODebounce = _data.CurrentPanel.PanelConfig.IODebounce, 
                                                MCPDebounce = _data.CurrentPanel.PanelConfig.MCPDebounce
                                            },
                                            Cultures.Resources.AL3_Code);
        }
        

        public bool DataEquals(XfpData otherData) => _data.CurrentPanel.LoopConfig.Equals(otherData.CurrentPanel.LoopConfig);

        
        public bool HasErrorsOrWarnings() => _data.CurrentPanel.LoopConfig.HasErrorsOrWarnings();
        public bool HasErrors()           => _data.CurrentPanel.LoopConfig.HasErrors();
        public bool HasWarnings()         => _data.CurrentPanel.LoopConfig.HasWarnings();
        #endregion


        #region IXfpDevicesViewModel implementation
        public void InitDeviceSelector() => InitMenu?.Invoke(DeviceSelectorSettings.Menu);
        #endregion


        #region Panel comms receive-data handlers
        private bool deviceReceived(object data)
        {
            //LoopConfigData.DetectedLoops is updated by the panel 'heartbeat'
            //comms, so will reflect what is actually connected
            if (LoopConfigData.DetectedLoops is not null)
            {
                NumLoops = (int)LoopConfigData.DetectedLoops;
                OnPropertyChanged(nameof(LoopIsFitted));
            }

            if (data is not DeviceData device
             || device.Index < 0
             || device.Index >= (device.LoopNum > 0 ? Loop2.Count : Loop1.Count)
             || device.LoopNum < 0 || device.LoopNum > 1)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Loop_x_Device_y_Type_z, 
                                                            device.LoopNum + 1, 
                                                            device.Index + 1, 
                                                            DeviceTypes.DeviceTypeName(device.DeviceType, DeviceTypes.CurrentProtocolType)??Cultures.Resources.Not_Fitted));
            
            if (device.LoopNum > 0)
            {
                setDevice(Loop2[device.Index], device);
                _data.CurrentPanel.LoopConfig.Loop2.Devices[device.Index] = device;
            }
            else
            {
                setDevice(Loop1[device.Index], device);
                _data.CurrentPanel.LoopConfig.Loop1.Devices[device.Index] = device;
            }

            return true;
        }


        private void setDevice(DeviceItemViewModel item, DeviceData device)
        {
            item.LoopNum = device.LoopNum;
            item.Index = device.Index;
            item.DeviceType = device.DeviceType;
            item.ZoneIndex = device.Zone;
            item.GroupIndex = device.Group;
            item.DeviceNameIndex = device.NameIndex;
            item.DaySensitivity = device.DaySensitivity;
            item.NightSensitivity = device.NightSensitivity;
            item.DayVolume = device.DayVolume;
            item.NightVolume = device.NightVolume;
            item.DayMode = device.DayMode;
            item.NightMode = device.NightMode;
            item.AncillaryBaseSounderGroup = device.AncillaryBaseSounderGroup;
            item.RemoteLEDEnabled = device.RemoteLEDEnabled;

            for (int i = 0; i < item.IOConfigItems.Count; i++)
            {
                item.IOConfigItems[i].Index = device.IOConfig[i].Index;
                item.IOConfigItems[i].DeviceType = device.DeviceType;
                item.IOConfigItems[i].InputOutput = device.IOConfig[i].InputOutput;
                item.IOConfigItems[i].Channel = device.IOConfig[i].Channel;
                item.IOConfigItems[i].ZoneGroupSet = device.IOConfig[i].ZoneGroupSet;
                item.IOConfigItems[i].NameIndex = device.IOConfig[i].NameIndex;
            }

            item.RefreshView();
        }

        private bool baseSounderGroupReceived(object data)
        {
            if (!DeviceTypes.CurrentProtocolIsXfpApollo)
                return true;

            if (data is not DeviceData device
             || device.Index < 0
             || device.LoopNum < 0 || device.LoopNum > 1)
                return false;

            if (device.Index >= (device.LoopNum > 0 ? Loop2.Count : Loop1.Count))
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Loop_x_Base_Sounder_Group_y, device.LoopNum + 1, device.Index + 1));

            if (device.LoopNum > 0)
            {
                Loop2[device.Index].DeviceData.AncillaryBaseSounderGroup = device.AncillaryBaseSounderGroup;
                _data.CurrentPanel.LoopConfig.Loop2.Devices[device.Index].AncillaryBaseSounderGroup = device.AncillaryBaseSounderGroup;
            }
            else
            {
                Loop1[device.Index].DeviceData.AncillaryBaseSounderGroup = device.AncillaryBaseSounderGroup;
                _data.CurrentPanel.LoopConfig.Loop1.Devices[device.Index].AncillaryBaseSounderGroup = device.AncillaryBaseSounderGroup;
            }

            return true;
        }


        private bool deviceNameReceived(object data)
        {
            //null indicates the last item has been received, so return True
            if (data is null)
                return true;

            if (data is not IndexedText deviceName)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Device_x_Name_y, deviceName.Index + 1, deviceName.Value));

            //_data.DeviceNamesConfig.Add(deviceName.Index, deviceName.Value);
            _data.CurrentPanel.DeviceNamesConfig.Add(deviceName.Value);
            return true;
        }

        private bool al3CodeReceived(object data)
        {
            if (data is not PanelConfigData.AL3CodeBundle al3Bundle)
                return false;

            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Alarm_Verification_Counts);

            _data.CurrentPanel.PanelConfig.AL3Code          = al3Bundle.AL3Code;
            _data.CurrentPanel.PanelConfig.BlinkPollingLED  = al3Bundle.BlinkPollingLED;
            _data.CurrentPanel.PanelConfig.MCPDebounce      = al3Bundle.MCPDebounce;
            _data.CurrentPanel.PanelConfig.IODebounce       = al3Bundle.IODebounce;
            _data.CurrentPanel.PanelConfig.DetectorDebounce = al3Bundle.DetectorDebounce;
            return true;
        }

        private bool zoneNameReceived(object data)
        {
            try
            {
                if (data is not IndexedText zone
                 || zone.Index < 0
                 || zone.Index >= ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                    return false;

                CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Zone_x_Name_y, zone.Index + 1, zone.Value));

                //if (zone.Index >= PanelConfigData.NumZonePanels)
                //    _data.NetworkConfig.RepeaterSettings.Repeaters[zone.Index - PanelConfigData.NumZonePanels].Name = zone.Value;
                //else
                //    _data.ZoneConfig.Zones[zone.Index].Name = zone.Value;
                if (zone.Index < ZoneConfigData.NumZones)
                    _data.CurrentPanel.ZoneConfig.Zones[zone.Index].Name = zone.Value;
                else if (zone.Index < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                    _data.CurrentPanel.ZonePanelConfig.Panels[zone.Index - ZoneConfigData.NumZones].Name = zone.Value;

            }catch (Exception ex) { CTecUtil.Debug.WriteLine(ex.ToString()); }
            return true;
        }


        private void downloadRequestsCompleted()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                RefreshView();

            }), DispatcherPriority.ContextIdle);
        }

        private void uploadRequestsCompleted() { }
        #endregion


        #region printing
        //public enum OrderBy
        //{
        //    ByDevice,
        //    ByGroup,
        //    ByZone,
        //}

        public override bool PrintPage(XpsDocumentWriter documentWriter) => PrintPage(documentWriter, SortOrder.Number);
        
        public bool PrintPage(XpsDocumentWriter documentWriter, SortOrder printOrder)
        {
            return true;
        }
        #endregion
    }
}
