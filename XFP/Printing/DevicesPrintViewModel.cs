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
using Xfp.ViewModels.PanelTools;
using Windows.UI;


namespace Xfp.Printing
{
    public class DevicesPrintViewModel : DeviceDetailsViewModel
    {
        public DevicesPrintViewModel(FrameworkElement parent, XfpData data, int panelNumber, SortOrder sortOrder, bool showOnlyFittedDevices) : base(parent, null)
        {
            PanelNumber = panelNumber;
            _sortOrder = sortOrder;
            ShowOnlyFittedDevices = showOnlyFittedDevices;

            InitMenu = new((c) => { });
            LoopChanged = new((l) => { });
            
            SetCulture(CultureInfo.CurrentCulture);

            PopulateView(data);
        }


        private SortOrder _sortOrder = SortOrder.Number;


        public new void PopulateView(XfpData data)
        {
            base.PopulateView(data);

            SortLoop(0);
            SortLoop(1);
        }


        public void SortLoop(int loop)
        {
            var deviceSort = new List<DeviceItemViewModel>(Loops[loop]);

            if (_sortOrder == SortOrder.Type)
                deviceSort.Sort(compareByDeviceType);
            else if (_sortOrder == SortOrder.ZoneGroupSet)
                deviceSort.Sort(compareByZoneGroupSet);

            Loops[loop] = new();
            foreach (var d in deviceSort)
                Loops[loop].Add(new DeviceItemViewModel() { DeviceData = new(d.DeviceData) });
        }

        private int compareByDeviceType(DeviceItemViewModel d1, DeviceItemViewModel d2)
        {
            var validD1 = DeviceTypes.IsValidDeviceType(d1.DeviceType, DeviceTypes.CurrentProtocolType);
            var validD2 = DeviceTypes.IsValidDeviceType(d2.DeviceType, DeviceTypes.CurrentProtocolType);

            if (!validD1 && !validD2) return d1.Index.CompareTo(d2.Index);
            if (!validD1) return 1;
            if (!validD2) return -1;

            return DeviceTypes.DeviceTypeName(d1.DeviceType, DeviceTypes.CurrentProtocolType).CompareTo(DeviceTypes.DeviceTypeName(d2.DeviceType, DeviceTypes.CurrentProtocolType));
        }

        private int compareByZoneGroupSet(DeviceItemViewModel d1, DeviceItemViewModel d2)
        {
            var validD1 = DeviceTypes.IsValidDeviceType(d1.DeviceType, DeviceTypes.CurrentProtocolType);
            var validD2 = DeviceTypes.IsValidDeviceType(d2.DeviceType, DeviceTypes.CurrentProtocolType);

            if (!validD1 && !validD2) return d1.Index.CompareTo(d2.Index);
            if (!validD1) return 1;
            if (!validD2) return -1;

            if (d1.IsGroupedDevice && d2.IsGroupedDevice) return d1.Group.CompareTo(d2.Group);
            if (d1.IsZonalDevice   && d2.IsZonalDevice)   return d1.Zone.CompareTo(d2.Zone);
            if (d1.IsGroupedDevice && d2.IsZonalDevice)   return d1.Group > 0 && d2.Zone == 0 ? 1 : -1;
            if (d1.IsZonalDevice   && d2.IsGroupedDevice) return d1.Zone == 0 && d2.Group > 0 ? -1 : 1;
            if (d1.IsGroupedDevice && d2.IsIODevice)      return -1;
            if (d1.IsIODevice      && d2.IsGroupedDevice) return 1;
            if (d1.IsZonalDevice   && d2.IsIODevice)      return -1;
            if (d1.IsIODevice      && d2.IsGroupedDevice) return 1;

            return getIOSortZGS(d1, d2).CompareTo(getIOSortZGS(d2, d1));
        }

        private int getIOSortZGS(DeviceItemViewModel d1, DeviceItemViewModel d2)
        {
            if (d1.IsIODevice)
            {
                for (int i = 0; i < DeviceData.NumIOSettings; i++)
                {
                    if (i >= d1.IOConfigItems.Count) return int.MaxValue;
                    if (i >= d2.IOConfigItems.Count) return -1;
                    if (d1.IOConfigItems[i].InputOutput == IOTypes.NotUsed && d2.IOConfigItems[i].InputOutput != IOTypes.NotUsed) return int.MaxValue;
                    //if (d1.IOConfig[i].InputOutput != IOTypes.NotUsed && d2.IOConfig[i].InputOutput == IOTypes.NotUsed) return -1;
                    if (d2.IOConfigItems[i].InputOutput == IOTypes.NotUsed) return -1;

                    int comp;

                    if (i == 0)
                    {
                        var d1Grouped = DeviceTypes.IOOutputIsGrouped(d1.DeviceType, DeviceTypes.CurrentProtocolType);
                        var d2Grouped = DeviceTypes.IOOutputIsGrouped(d2.DeviceType, DeviceTypes.CurrentProtocolType);

                        if (d1Grouped && !d2Grouped) return -1;
                        if (!d1Grouped && d2Grouped) return 1;

                        if ((comp = d1.IOConfigItems[i].ZoneGroupSet.Value.CompareTo(d2.IOConfigItems[i].ZoneGroupSet)) == 0)
                            continue;

                        return comp > 0 ? int.MaxValue : comp;
                    }

                    if ((comp = d1.IOConfigItems[i].ZoneGroupSet.Value.CompareTo(d2.IOConfigItems[i].ZoneGroupSet)) == 0)
                        continue;

                    return comp > 0 ? int.MaxValue : comp;
                }
            }

            return 0;
        }
    }
}
