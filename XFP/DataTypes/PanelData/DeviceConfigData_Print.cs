using CTecDevices;
using CTecDevices.DeviceTypes;
using CTecDevices.Protocol;
using CTecUtil;
using CTecUtil.Printing;
using CTecUtil.UI.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Windows.Devices.Sms;
using Xfp.UI;
using Xfp.UI.ViewHelpers;
using static Xfp.ViewModels.PanelTools.DeviceItemViewModel;

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceConfigData
    {
        public void Print(FlowDocument doc, XfpData data, int panelNumber, bool printAllLoopDevices, SortOrder printOrder, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _printOrder = printOrder;
            var devicePage = new Section();
            devicePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1)));

            devicePage.Blocks.Add(deviceList(printAllLoopDevices));
            doc.Blocks.Add(devicePage);
        }
        //public List<Grid> Print2(XfpData data, int panelNumber, bool printAllLoopDevices, SortOrder printOrder)
        //{
        //    _printOrder = printOrder;

        //    var result = new List<Grid>();

        //    result.Add(PrintUtil.PageHeaderGrid(string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1)));

        //    result.Add(deviceList(printAllLoopDevices));
        //    return result;
        //}


        internal static DeviceNameGetter GetDeviceName;


        private SortOrder _printOrder;
        private int       _totalColumns;
        private int       _ioSettingsColumns = 5;
        private const int _ioSubheaderFontSize = 8;

        private static SolidColorBrush _seeIoSettingsForeground = Styles.SeeDetailsPrintBrush;
        private static SolidColorBrush _ioBorderBrush           = Styles.Brush05;
        private static SolidColorBrush _ioSubheaderBrush        = Styles.Brush02;

        
        private List<string> _defaultSubaddressNames = new() { "0", "1", "2", "3" };
        private List<string> _xfpHushSubaddressNames = new() { Cultures.Resources.Subaddress_Hush_0, 
                                                               Cultures.Resources.Subaddress_Hush_1, 
                                                               Cultures.Resources.Subaddress_Hush_2, 
                                                               Cultures.Resources.Subaddress_Hush_3 };
        
        public BlockUIContainer deviceList(bool printAllLoopDevices)
        {
            var grid = columnHeaders();

            int row = grid.RowDefinitions.Count;
            int col;
            int dataRows = 0;

            var deviceSort = new List<DeviceData>(Devices);

            if (_printOrder == SortOrder.Type)
                deviceSort.Sort(compareByDeviceType);
            else if (_printOrder == SortOrder.ZoneGroupSet)
                deviceSort.Sort(compareByZoneGroupSet);

            foreach (var d in deviceSort)
            {
                dataRows++;
                col = 0;

                if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    GridUtil.AddRowToGrid(grid);
                    grid.Children.Add(GridUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

//NB: need to know number of I/O settings and set RowSpan on the left columns; vert alignment=Top
//NB: alse Mode AND Volume devices need 2 rows
int ioRows = 1;

                    //number
                    grid.Children.Add(GridUtil.GridCell((d.Index + 1).ToString(), row, col++, ioRows, 1, HorizontalAlignment.Right, VerticalAlignment.Top));

                    if (DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                    {
                        //icon & type name
                        grid.Children.Add(GridUtil.GridCellImage(DeviceTypes.DeviceIcon(d.DeviceType, DeviceTypes.CurrentProtocolType), row, col++, ioRows, 1, 18, 18, HorizontalAlignment.Left, VerticalAlignment.Top));
                        grid.Children.Add(GridUtil.GridCell(DeviceTypes.DeviceTypeName(d.DeviceType, DeviceTypes.CurrentProtocolType), row, col++, ioRows, 1, HorizontalAlignment.Left, VerticalAlignment.Top));

                        //zone/group/set
                        if (d.IsIODevice)
                            //grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.See_IO_Configuration, row, col++, HorizontalAlignment.Left, PrintUtil.PrintDefaultFontSize, FontStyles.Italic), _seeIoSettingsForeground));
                            //grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell("See I/O  →", row, col++), _seeIoSettingsForeground));
                            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(" See I/O  ►", row, col++, ioRows, 1, HorizontalAlignment.Left, VerticalAlignment.Top), _seeIoSettingsForeground));
                        else
                            grid.Children.Add(GridUtil.GridCell(zgsDescription(false, d.IsGroupedDevice, false, d.IsGroupedDevice ? d.Group : d.Zone), row, col++, ioRows, 1, HorizontalAlignment.Left, VerticalAlignment.Top));

                        //name
                        grid.Children.Add(GridUtil.GridCell(GetDeviceName?.Invoke(d.NameIndex), row, col++, ioRows, 1, HorizontalAlignment.Left, VerticalAlignment.Top));

                        //volume/sensitivity/mode & day:night values
                        if (d.IsVolumeDevice)
                        {
                            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Volume, row, col++, HorizontalAlignment.Left, VerticalAlignment.Top));
                            grid.Children.Add(GridUtil.GridCell(string.Format("{0}:{1}", d.DayVolume, d.NightVolume ?? 0), row, col++, HorizontalAlignment.Center, VerticalAlignment.Top));
                        }
                        else if (d.IsSensitivityDevice)
                        {
                            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Sensitivity, row, col++, HorizontalAlignment.Left, VerticalAlignment.Top));
                            grid.Children.Add(GridUtil.GridCell(string.Format("{0}:{1}", d.DaySensitivity ?? 0, d.NightSensitivity ?? 0), row, col++, HorizontalAlignment.Center, VerticalAlignment.Top));
                        }
                        else
                        {
                            col += 2;
                        }

                        if (DeviceTypes.CurrentProtocolIsXfpApollo)
                        {
                            //remote LED
                            grid.Children.Add(GridUtil.GridCellBool(d.RemoteLEDEnabled ?? false, row, col++, ioRows,  1, false, false, HorizontalAlignment.Center, VerticalAlignment.Top));

                            //base sounder group
                            grid.Children.Add(GridUtil.GridCell(d.AncillaryBaseSounderGroup is null ? "--" : string.Format(Cultures.Resources.Group_x, d.AncillaryBaseSounderGroup.Value), row, col++, ioRows, 1, HorizontalAlignment.Left, VerticalAlignment.Top));
                        }

                        if (d.IsIODevice)
                        {
                            List<string> subaddressNames = DeviceTypes.CurrentProtocolIsXfpCast && d.DeviceType == (int)XfpCastDeviceTypeIds.HS2
                                                            ? _xfpHushSubaddressNames
                                                            : _defaultSubaddressNames;

                            int ioRow = row;
                            int newRows = 0;

                            for (int i = 0; i < d.IOConfig.Count; i++)
                            {
                                if (d.IOConfig[i].InputOutput != IOTypes.NotUsed)
                                {
                                    int ioCol = col;

                                    if (ioRow > row)
                                    {
                                        GridUtil.AddRowToGrid(grid);
                                        grid.Children.Add(GridUtil.GridBackground(ioRow, 0, 1, _totalColumns, Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                                        newRows++;
                                    }

                                    var isGroup = d.IOConfig[i].InputOutput == IOTypes.Output && d.IsGroupedDevice;
                                    var isSet   = d.IOConfig[i].InputOutput == IOTypes.Output && !d.IsZonalDevice;

                                    if (d.IOConfig[i].Index >= 0 && d.IOConfig[i].Index < subaddressNames.Count)
                                        grid.Children.Add(GridUtil.GridCell(subaddressNames[d.IOConfig[i].Index], ioRow, ioCol, HorizontalAlignment.Left, VerticalAlignment.Top));
                                    
                                    ioCol++;

                                    grid.Children.Add(GridUtil.GridCell(CTecDevices.Enums.IOTypeToString(d.IOConfig[i].InputOutput), ioRow, ioCol++, HorizontalAlignment.Left, VerticalAlignment.Top));
                                    grid.Children.Add(GridUtil.GridCell(((d.IOConfig[i].Channel ?? 0) + 1).ToString(), ioRow, ioCol++, HorizontalAlignment.Left, VerticalAlignment.Top));
                                    grid.Children.Add(GridUtil.GridCell(zgsDescription(true, isGroup, isSet, (int)d.IOConfig[i].ZoneGroupSet), ioRow, ioCol++, HorizontalAlignment.Left, VerticalAlignment.Top));
                                    grid.Children.Add(GridUtil.GridCell(GetDeviceName?.Invoke(d.IOConfig[i].NameIndex), ioRow, ioCol++, HorizontalAlignment.Left, VerticalAlignment.Top));
                                    ioRow++;
                                }
                            }

                            row += newRows;
                        }
                    }
                }
                    
                row++;
            }

            GridUtil.AddRowToGrid(grid, 10);

            return new(grid);
        }


        private Grid columnHeaders()
        {
            //measure required column widths for subaddress and IO headers

            var cellMargins      = (int)(PrintUtil.DefaultGridMargin.Left + PrintUtil.DefaultGridMargin.Right) + 1;
            
            var subaddressHeader = DeviceTypes.CurrentProtocolIsXfpCast ? Cultures.Resources.Subaddress : Cultures.Resources.Subaddress_Abbr;
            var subaddressWidth  = (int)FontUtil.MeasureText(subaddressHeader, new(PrintUtil.PrintDefaultFont), _ioSubheaderFontSize, FontStyles.Italic, FontWeights.Normal, FontStretches.Normal).Width + 1;
            foreach (var s in DeviceTypes.CurrentProtocolIsXfpCast ? _xfpHushSubaddressNames : _defaultSubaddressNames)
            {
                var wSub = (int)FontUtil.MeasureText(s, new(PrintUtil.PrintDefaultFont), PrintUtil.PrintDefaultFontSize, FontStyles.Italic, FontWeights.Normal, FontStretches.Normal).Width + 1;
                if (wSub > subaddressWidth) subaddressWidth = wSub;
            }

            subaddressWidth += cellMargins;

            var wIn  = (int)FontUtil.MeasureText(Cultures.Resources.Input,  new(PrintUtil.PrintDefaultFont), PrintUtil.PrintDefaultFontSize, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal).Width + 1;
            var wOut = (int)FontUtil.MeasureText(Cultures.Resources.Output, new(PrintUtil.PrintDefaultFont), PrintUtil.PrintDefaultFontSize, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal).Width + 1;
            var ioWidth = Math.Max(wIn, wOut);

            ioWidth += cellMargins;


            Grid grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            _totalColumns = 0;
            GridUtil.AddColumnToGrid(grid);             _totalColumns++;        // num
            GridUtil.AddColumnToGrid(grid);             _totalColumns++;        // icon
            GridUtil.AddColumnToGrid(grid, 140);        _totalColumns++;        // type name
            GridUtil.AddColumnToGrid(grid, 100);        _totalColumns++;        // z/g/s
            GridUtil.AddColumnToGrid(grid);             _totalColumns++;        // name
            GridUtil.AddColumnToGrid(grid);             _totalColumns++;        // v/s/m
            GridUtil.AddColumnToGrid(grid);             _totalColumns++;        // day:night
            if (DeviceTypes.CurrentProtocolIsXfpApollo) 
            {
                GridUtil.AddColumnToGrid(grid);         _totalColumns++;        // remote LED
                GridUtil.AddColumnToGrid(grid);         _totalColumns++;        // base sounder grp
            }
            GridUtil.AddColumnToGrid(grid, subaddressWidth); _totalColumns++;   // subaddress
            GridUtil.AddColumnToGrid(grid, ioWidth);         _totalColumns++;   // i/o
            GridUtil.AddColumnToGrid(grid);                  _totalColumns++;   // chan
            GridUtil.AddColumnToGrid(grid, 80);              _totalColumns++;   // z/g/s
            GridUtil.AddColumnToGrid(grid);                  _totalColumns++;   // name


            grid.Children.Add(GridUtil.GridBackground(0, 0, 3, _totalColumns, PrintUtil.GridHeaderBackground));

            int col = 0;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Number_Symbol,           0, col++, 3, 1, HorizontalAlignment.Right, VerticalAlignment.Top));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Device_Type,             0, col++, 3, 2, HorizontalAlignment.Left, VerticalAlignment.Top));
            col++;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Zone_Group,              0, col++, 3, 1, HorizontalAlignment.Left, VerticalAlignment.Top));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Device_Name,             0, col++, 3, 1, HorizontalAlignment.Left, VerticalAlignment.Top));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Volume_Sensitivity_mode, 0, col++, 3, 1, HorizontalAlignment.Left, VerticalAlignment.Top));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Day_Night,               0, col++, 3, 1, HorizontalAlignment.Center, VerticalAlignment.Top));

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Remote_LED_Header,   0, col++, 3, 1, HorizontalAlignment.Center, VerticalAlignment.Top));
                grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Base_Sounder_Header, 0, col++, 3, 1, HorizontalAlignment.Left, VerticalAlignment.Top));
            }

            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.IO_Configuration,        0, col, 1, _ioSettingsColumns, HorizontalAlignment.Center, VerticalAlignment.Top));
            GridUtil.AddBorderToGrid(grid, 1, col, 1, 6, _ioBorderBrush, new Thickness(0, 1, 0, 0), new CornerRadius(0, 0, 0, 0), 2);

            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(subaddressHeader,                  2, col++, 1, 2, false, HorizontalAlignment.Left, _ioSubheaderFontSize, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.I_O,            2, col++, 1, 2, false, HorizontalAlignment.Left, _ioSubheaderFontSize, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.Channel_Abbr,   2, col++, 1, 1, false, HorizontalAlignment.Left, _ioSubheaderFontSize, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.Zone_Group_Set, 2, col++, 1, 1, false, HorizontalAlignment.Left, _ioSubheaderFontSize, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridCell(Cultures.Resources.Description,    2, col++, 1, 1, false, HorizontalAlignment.Left, _ioSubheaderFontSize, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));

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
