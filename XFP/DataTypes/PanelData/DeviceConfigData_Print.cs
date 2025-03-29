using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CTecDevices.Protocol;
using CTecUtil.Printing;
using System;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using Xfp.UI.ViewHelpers;
using static Xfp.ViewModels.PanelTools.DeviceItemViewModel;
using System.Linq;
using System.Collections.Generic;
using CTecDevices;

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceConfigData
    {
        public void Print(FlowDocument doc, int panelNumber, bool printAllLoopDevices, LoopPrintOrder printOrder)
        {
            _printOrder = printOrder;
            var devicePage = new Section();
            devicePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1)));

            devicePage.Blocks.Add(deviceList(printAllLoopDevices));
            doc.Blocks.Add(devicePage);
        }


        internal static DeviceNameGetter GetDeviceName;


        private int _totalColumns = 14;
        private int _ioSettingsColumns = 5;
        private static SolidColorBrush _seeIoSettingsForeground = (SolidColorBrush)Application.Current.TryFindResource("BrushSeeIOPrint");
        private static SolidColorBrush _ioSubheaderBrush        = (SolidColorBrush)Application.Current.TryFindResource("Brush01");
        private LoopPrintOrder _printOrder;


        public BlockUIContainer deviceList(bool printAllLoopDevices)
        {
            var grid = columnHeaders();

            int row = grid.RowDefinitions.Count;
            int col;
            int dataRows = 0;

            var deviceSort = new List<DeviceData>(Devices);

            if (_printOrder == LoopPrintOrder.ByDeviceType)
                deviceSort.Sort(compareByDeviceType);
            else if (_printOrder == LoopPrintOrder.ByGroupZone)
                deviceSort.Sort(compareByZoneGroupSet);

            foreach (var d in deviceSort)
            {
                dataRows++;
                col = 0;

                if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    GridUtil.AddRowToGrid(grid);
                    grid.Children.Add(GridUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                    grid.Children.Add(GridUtil.GridCell(d.Index + 1, row, col++, HorizontalAlignment.Right));
                }

                if (DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    grid.Children.Add(GridUtil.GridCellImage(DeviceTypes.DeviceIcon(d.DeviceType, DeviceTypes.CurrentProtocolType), row, col++, 18, 18));
                    grid.Children.Add(GridUtil.GridCell(DeviceTypes.DeviceTypeName(d.DeviceType, DeviceTypes.CurrentProtocolType), row, col++));

                    if (d.IsIODevice)
                    {
                        grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.See_IO_Configuration, row, col++, HorizontalAlignment.Left, PrintUtil.PrintDefaultFontSize, FontStyles.Italic), _seeIoSettingsForeground));
                    }
                    else
                    {
                        grid.Children.Add(GridUtil.GridCell(zgsDescription(false, d.IsGroupedDevice, false, d.IsGroupedDevice ? d.Group : d.Zone), row, col++));
                    }

                    grid.Children.Add(GridUtil.GridCell(GetDeviceName?.Invoke(d.NameIndex), row, col++));
                    if (d.IsVolumeDevice)
                    {
                        grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Volume, row, col++));
                        grid.Children.Add(GridUtil.GridCell(string.Format("{0}:{1}", d.DayVolume, d.NightVolume ?? 0), row, col++, HorizontalAlignment.Center));
                    }
                    else if (d.IsSensitivityDevice)
                    {
                        grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Sensitivity, row, col++));
                        grid.Children.Add(GridUtil.GridCell(string.Format("{0}:{1}", d.DaySensitivity ?? 0, d.NightSensitivity ?? 0), row, col++, HorizontalAlignment.Center));
                    }
                    else
                    {
                        col += 2;
                    }

                    if (DeviceTypes.CurrentProtocolIsXfpApollo)
                    {
                        grid.Children.Add(GridUtil.GridCellBool(d.RemoteLEDEnabled ?? false, row, col++, false, false));
                        grid.Children.Add(GridUtil.GridCell(d.AncillaryBaseSounderGroup is null ? "--" : string.Format(Cultures.Resources.Group_x, d.AncillaryBaseSounderGroup.Value), row, col++));
                    }
                    else
                    {
                        col += 2;
                    }

                    if (d.IsIODevice)
                    {
                        //grid.Children.Add(ioSettings(d, row, col++));

                        int ioRow = row;
                        int newRows = 0;

                        for (int i = 0; i < d.IOConfig.Count; i++)
                        {
                            if (d.IOConfig[i].InputOutput != CTecDevices.IOTypes.NotUsed)
                            {
                                int ioCol = col;

                                if (ioRow > row)
                                {
                                    GridUtil.AddRowToGrid(grid);
                                    grid.Children.Add(GridUtil.GridBackground(ioRow, 0, 1, _totalColumns, Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                                    newRows++;
                                }

                                var isGroup = d.IOConfig[i].InputOutput == CTecDevices.IOTypes.Output && d.IsGroupedDevice;
                                var isSet   = d.IOConfig[i].InputOutput == CTecDevices.IOTypes.Output && !d.IsZonalDevice;

                                grid.Children.Add(GridUtil.GridCell(d.IOConfig[i].Index + 1, ioRow, ioCol++));
                                grid.Children.Add(GridUtil.GridCell(CTecDevices.Enums.IOTypeToString(d.IOConfig[i].InputOutput), ioRow, ioCol++));
                                grid.Children.Add(GridUtil.GridCell((d.IOConfig[i].Channel ?? 0) + 1, ioRow, ioCol++));
                                grid.Children.Add(GridUtil.GridCell(zgsDescription(true, isGroup, isSet, (int)d.IOConfig[i].ZoneGroupSet), ioRow, ioCol++));
                                grid.Children.Add(GridUtil.GridCell(GetDeviceName?.Invoke(d.IOConfig[i].NameIndex), ioRow, ioCol++));
                                ioRow++;
                            }
                        }

                        row += newRows;
                    }
                }
                    
                row++;
            }

            GridUtil.AddRowToGrid(grid, 10);

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalColumns; i++)
                GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridBackground(0, 0, 2, _totalColumns, PrintUtil.GridHeaderBackground));

            int col = 0;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Number_Symbol,           0, col++, HorizontalAlignment.Right));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Device_Type,             0, col, 1, 2));
            col += 2;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Zone_Group,              0, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Device_Name,             0, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Volume_Sensitivity_mode, 0, col++, 2, 1));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Day_Night,               0, col++));

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Remote_LED_Header,   0, col++));
                grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Base_Sounder_Header, 0, col++));
            }

            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.IO_Configuration,        0, col, 1, _ioSettingsColumns));
            
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.I_O,            1, col++, 1, 2, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            col++;
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.Channel_Abbr,   1, col++, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.Zone_Group_Set, 1, col++, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.Description,    1, col++, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));

            return grid;
        }
        

        private string zgsDescription(bool isIODevice, bool isGroupedDevice, bool isSetDevice, int value)
        {
            if (value == 0)
                return Cultures.Resources.Use_In_Special_C_And_E;

            if (isGroupedDevice)
                return string.Format(Cultures.Resources.Group_x, value);

            if (isIODevice && isSetDevice) 
                return string.Format(Cultures.Resources.Set_x, value);

            return string.Format(Cultures.Resources.Zone_x, value);
        }


        private int compareByDeviceType(DeviceData d1, DeviceData d2)
        {
            var validD1 = DeviceTypes.IsValidDeviceType(d1.DeviceType, DeviceTypes.CurrentProtocolType);
            var validD2 = DeviceTypes.IsValidDeviceType(d2.DeviceType, DeviceTypes.CurrentProtocolType);

            if (!validD1 && !validD2) return d1.Index.CompareTo(d2.Index);
            if (!validD1) return 1;
            if (!validD2) return -1;

            return DeviceTypes.DeviceTypeName(d1.DeviceType, DeviceTypes.CurrentProtocolType).CompareTo(DeviceTypes.DeviceTypeName(d2.DeviceType, DeviceTypes.CurrentProtocolType));
        }

        private int compareByZoneGroupSet(DeviceData d1, DeviceData d2)
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


        private int getIOSortZGS(DeviceData d1, DeviceData d2)
        {
            if (d1.IsIODevice)
            {
                for (int i = 0; i < DeviceData.NumIOSettings; i++)
                {
                    if (i >= d1.IOConfig.Count) return int.MaxValue;
                    if (i >= d2.IOConfig.Count) return -1;
                    if (d1.IOConfig[i].InputOutput == IOTypes.NotUsed && d2.IOConfig[i].InputOutput != IOTypes.NotUsed) return int.MaxValue;
                    //if (d1.IOConfig[i].InputOutput != IOTypes.NotUsed && d2.IOConfig[i].InputOutput == IOTypes.NotUsed) return -1;
                    if (d2.IOConfig[i].InputOutput == IOTypes.NotUsed) return -1;

                    int comp;

                    if (i == 0)
                    {
                        var d1Grouped = DeviceTypes.IOOutputIsGrouped(d1.DeviceType, DeviceTypes.CurrentProtocolType);
                        var d2Grouped = DeviceTypes.IOOutputIsGrouped(d2.DeviceType, DeviceTypes.CurrentProtocolType);

                        if (d1Grouped && !d2Grouped) return -1;
                        if (!d1Grouped && d2Grouped) return 1;

                        if ((comp = d1.IOConfig[i].ZoneGroupSet.Value.CompareTo(d2.IOConfig[i].ZoneGroupSet)) == 0)
                            continue;

                        return comp > 0 ? int.MaxValue : comp;
                    }

                    if ((comp = d1.IOConfig[i].ZoneGroupSet.Value.CompareTo(d2.IOConfig[i].ZoneGroupSet)) == 0)
                        continue;

                    return comp > 0 ? int.MaxValue : comp;
                }
            }

            return 0;
        }
    }
}
