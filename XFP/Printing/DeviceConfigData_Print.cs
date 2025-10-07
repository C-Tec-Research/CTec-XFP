using CTecDevices;
using CTecDevices.DeviceTypes;
using CTecDevices.Protocol;
using CTecUtil;
using CTecUtil.Printing;
using CTecUtil.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xfp.UI;
using static Xfp.ViewModels.PanelTools.DeviceItemViewModel;

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceConfigData
    {
        public void Print(FlowDocument doc, XfpData data, int panelNumber, bool printAllLoopDevices, SortOrder printOrder, CTecUtil.PrintActions printAction)
        {
            _doc = doc;
            _data = data;

            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize-2);
            TableUtil.SetFontStretch(PrintUtil.FontNarrowWidth);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(PrintUtil.DefaultGridMargin);

            //var reportName = string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1);
            //var table = new Table() { Name = "Loop1Devices", Tag = reportName + _pageNum };
            //setColumnHeaders(table, reportName);

            
            _deviceSort = new List<DeviceData>(_data.Panels[1].Loop1Config.Devices);

            if (!printAllLoopDevices)
                for (int i = _deviceSort.Count - 1; i > 0; i--)
                    if (!DeviceTypes.IsValidDeviceType(_deviceSort[i].DeviceType, DeviceTypes.CurrentProtocolType))
                        _deviceSort.RemoveAt(i);

            if (_printOrder == SortOrder.Type)
                _deviceSort.Sort(compareByDeviceType);
            else if (_printOrder == SortOrder.ZoneGroupSet)
                _deviceSort.Sort(compareByZoneGroupSet);

            
            int rowCount = 0;

            while (deviceList(printAllLoopDevices, ref rowCount))
                ;

            //doc.Blocks.Add(table);
        }


        private XfpData _data;
        List<DeviceData> _deviceSort;
        internal static DeviceNamesEntryGetter GetDeviceName;


        private SortOrder _printOrder;
        private int       _totalColumns;
        private int       _leftColumns;
        private int       _ioSettingsColumns = 5;
       
        private FlowDocument _doc;
        private int          _pageNum = 0;

        private static SolidColorBrush _seeIoSettingsForeground = Styles.SeeDetailsPrintBrush;
        private static SolidColorBrush _ioBorderBrush           = Styles.Brush06;
        private static SolidColorBrush _ioSubheaderBrush        = Styles.Brush02;

        
        private readonly List<string> _defaultSubaddressNames = new() { "0", "1", "2", "3" };
        private readonly List<string> _xfpHushSubaddressNames = new() { Cultures.Resources.Subaddress_Hush_0, 
                                                                        Cultures.Resources.Subaddress_Hush_1, 
                                                                        Cultures.Resources.Subaddress_Hush_2, 
                                                                        Cultures.Resources.Subaddress_Hush_3 };
        


        private void setColumnHeaders(Table table, string reportHeader)
        {
            //measure required column widths for subaddress and IO headers
            var cellMargins      = (int)(PrintUtil.DefaultGridMargin.Left + PrintUtil.DefaultGridMargin.Right) + 1;
            
            var subaddressHeader = Cultures.Resources.Subaddress_Abbr;
            var subaddressWidth  = (int)TableUtil.MeasureText(subaddressHeader).Width + 1;
            foreach (var s in DeviceTypes.CurrentProtocolIsXfpCast ? _xfpHushSubaddressNames : _defaultSubaddressNames)
            {
                var wSub = (int)TableUtil.MeasureText(s).Width + 1;
                if (wSub > subaddressWidth) subaddressWidth = wSub;
            }
            subaddressWidth += cellMargins;

            var wIn  = (int)TableUtil.MeasureText(Cultures.Resources.Input).Width + 1;
            var wOut = (int)TableUtil.MeasureText(Cultures.Resources.Output).Width + 1;
            var ioWidth = Math.Max(wIn, wOut);
            ioWidth += cellMargins;
            
            var wType = 100;
            var wZSG  = DeviceTypes.CurrentProtocolIsXfpApollo ? 40 : 70;
            var wName = TableUtil.MeasureText(new string('n', DeviceNamesConfigData.DeviceNameLength)).Width + 1;   //no particular reason for 'n'
            var wChan = TableUtil.MeasureText(Cultures.Resources.Channel_Abbr).Width + 1;

            //define table's columns
            _totalColumns = 0;
            table.Columns.Add(new TableColumn() { Width = new GridLength(18) });              _totalColumns++;    // num
            table.Columns.Add(new TableColumn() { Width = new GridLength(30) });              _totalColumns++;    // icon
            table.Columns.Add(new TableColumn() { Width = new GridLength(wType) });           _totalColumns++;    // type name
            table.Columns.Add(new TableColumn() { Width = new GridLength(wZSG) });            _totalColumns++;    // z/g/s
            table.Columns.Add(new TableColumn() { Width = new GridLength(wName) });           _totalColumns++;    // name
            table.Columns.Add(new TableColumn() { Width = new GridLength(40) });              _totalColumns++;    // v/s/m
            table.Columns.Add(new TableColumn() { Width = new GridLength(40) });              _totalColumns++;    // day:night

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                table.Columns.Add(new TableColumn() { Width = new GridLength(50) });          _totalColumns++;    // remote LED
                table.Columns.Add(new TableColumn() { Width = new GridLength(80) });          _totalColumns++;    // base sounder grp
            }
            _leftColumns = _totalColumns;

            table.Columns.Add(new TableColumn() { Width = new GridLength(subaddressWidth) }); _totalColumns++;    // subaddress
            table.Columns.Add(new TableColumn() { Width = new GridLength(ioWidth) });         _totalColumns++;    // i/o
            table.Columns.Add(new TableColumn() { Width = new GridLength(wChan) });           _totalColumns++;    // chan
            table.Columns.Add(new TableColumn() { Width = new GridLength(wZSG) });            _totalColumns++;    // z/g/s
            table.Columns.Add(new TableColumn() { Width = new GridLength(100) });             _totalColumns++;    // name


            //add header row group for the header
            var headerGroup = new TableRowGroup();
            
            var headerRow1 = new TableRow();
            var headerRow2 = new TableRow();
            var headerRow3 = new TableRow();

            headerRow1.Background = headerRow2.Background = headerRow3.Background = PrintUtil.GridHeaderBackground;
            
            var colHeader = TableUtil.NewCell(reportHeader, 1, _totalColumns, FontWeights.Bold);
            colHeader.FontSize = PrintUtil.PrintDefaultFontSize;
            colHeader.Padding = new(5,0,0,0);
            headerRow1.Cells.Add(colHeader);

            var colNum = TableUtil.NewCell(Cultures.Resources.Number_Symbol, 3, 1, TextAlignment.Right, FontWeights.Bold);
            colNum.Padding = new(0,0,5,0);
            headerRow2.Cells.Add(colNum);

            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Device_Type,             3, 2, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Zone_Group,              3, 1, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Device_Name,             3, 1, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Volume_Sensitivity_mode, 3, 1, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Day_Night,               3, 1, TextAlignment.Center, FontWeights.Bold));

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Remote_LED_Header,   3, 1, TextAlignment.Center,  FontWeights.Bold));
                headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Base_Sounder_Header, 3, 1, FontWeights.Bold));
            }
            
            headerRow3.Cells.Add(TableUtil.NewCell(subaddressHeader));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.I_O));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Channel_Abbr));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Zone_Group_Set_Abbr));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Description_Abbr));

            //add I/O config subheader & underline
            var ioSubhead = TableUtil.NewCell(Cultures.Resources.IO_Configuration, 1, _ioSettingsColumns, FontWeights.Bold);
            ioSubhead.BorderBrush = Styles.Brush06;
            ioSubhead.BorderThickness = new(0,0,0,0.25);
            headerRow2.Cells.Add(ioSubhead);


            headerGroup.Rows.Add(headerRow1);
            headerGroup.Rows.Add(headerRow2);
            headerGroup.Rows.Add(headerRow3);
            table.RowGroups.Add(headerGroup);
        }
        

        private int _deviceIndex = 0;

        public bool deviceList(bool printAllLoopDevices, ref int rowCount)
        {
            int dataRows = 0;
            
            var reportName = string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1);
            var table = new Table() { Name = "Loop1Devices", Tag = reportName + ++_pageNum };

            setColumnHeaders(table, reportName);

            try
            {
                var bodyGroup = new TableRowGroup();

                for (int ii = rowCount; ii < _deviceSort.Count; ii++)
                {
                    var dev = _deviceSort[ii];
                    dataRows++;

                    //find number of rows of Mode/Sensitivity/Volume values
                    var vsmRows = 0;
                    if (DeviceTypes.IsSensitivityDevice(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                        vsmRows++;
                    if (DeviceTypes.IsVolumeDevice(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                        vsmRows++;
                    if (DeviceTypes.IsModeDevice(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                        vsmRows++;
                    if (vsmRows == 0)
                        vsmRows = 1;

                    //find number of I/O settings
                    int ioRows = dev.IsIODevice ? dev.IOConfig.Count : 1;

                    //rows required depends on the above
                    var numRows = Math.Max(ioRows, vsmRows);

                    var tableRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground };

                    //device number
                    var colNum = TableUtil.NewCell((dev.Index + 1).ToString(), TextAlignment.Right);
                    colNum.Padding = new(0,0,5,0);
                    tableRow.Cells.Add(colNum);


                    if (DeviceTypes.IsValidDeviceType(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                    {
                        //device icon
                        var deviceIcon = new Image()
                        {
                            Source = DeviceTypes.DeviceIcon(dev.DeviceType, DeviceTypes.CurrentProtocolType), 
                            Width = 14, Height = 14, 
                            HorizontalAlignment = HorizontalAlignment.Left, 
                            VerticalAlignment = VerticalAlignment.Center, 
                            Margin = new(0) 
                        };

                        var cellIcon = new TableCell();
                        cellIcon.Blocks.Add(new BlockUIContainer(deviceIcon));
                        tableRow.Cells.Add(cellIcon);

                        //device type name
                        //tableRow.Cells.Add(TableUtil.NewCell(DeviceTypes.DeviceTypeName(d.DeviceType, DeviceTypes.CurrentProtocolType)));
                        var cellType = TableUtil.NewCell(DeviceTypes.DeviceTypeName(dev.DeviceType, DeviceTypes.CurrentProtocolType));
                            
                        tableRow.Cells.Add(cellType);

                        //zone/group/set
                        //if (d.IsIODevice)
                        //    tableRow.Cells.Add(TableUtil.NewCell("  " + Cultures.Resources.See_IO_Configuration_Abbr, _seeIoSettingsForeground, FontStyles.Italic));
                        //else
                        //    tableRow.Cells.Add(TableUtil.NewCell(zgsDescription(false, d.IsGroupedDevice, false, d.IsGroupedDevice ? d.Group : d.Zone)));
                        var cellZG = dev.IsIODevice ? TableUtil.NewCell("  " + Cultures.Resources.See_IO_Configuration_Abbr, _seeIoSettingsForeground, FontStyles.Italic)
                                                    : TableUtil.NewCell(zgsDescription(false, dev.IsGroupedDevice, false, dev.IsGroupedDevice ? dev.Group : dev.Zone));
                        tableRow.Cells.Add(cellZG);

                        //name
                        //tableRow.Cells.Add(TableUtil.NewCell(GetDeviceName?.Invoke(d.NameIndex)));
                        var cellName = TableUtil.NewCell(GetDeviceName?.Invoke(dev.NameIndex));
                        tableRow.Cells.Add(cellName);


                        //volume/sensitivity/mode & day:night values
                        int modeSensVolRows = 0;
                            
                        if (dev.IsModeDevice || dev.IsVolumeDevice || dev.IsSensitivityDevice)
                        {
                            var vsmText = new StringBuilder();
                            var dnText  = new StringBuilder();
                            if (dev.IsModeDevice)
                            {
                                //tableRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Mode));
                                //tableRow.Cells.Add(TableUtil.NewCell(string.Format("{0}:{1}", d.DayMode, d.NightMode ?? 0)));
                                vsmText.Append(Cultures.Resources.Mode);
                                dnText.Append(string.Format("{0}:{1}", dev.DayMode, dev.NightMode ?? 0));
                                modeSensVolRows++;
                            }

                            if (dev.IsVolumeDevice)
                            {
                                //tableRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Volume));
                                //tableRow.Cells.Add(TableUtil.NewCell(string.Format("{0}:{1}", d.DayVolume, d.NightVolume ?? 0)));
                                if (modeSensVolRows > 0)
                                {
                                    vsmText.Append("\n");
                                    dnText.Append("\n");
                                }
                                vsmText.Append(Cultures.Resources.Volume);
                                dnText.Append(string.Format("{0}:{1}", dev.DayVolume, dev.NightVolume ?? 0));
                                modeSensVolRows++;
                            }

                            if (dev.IsSensitivityDevice)
                            {
                                //tableRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Sensitivity));
                                //tableRow.Cells.Add(TableUtil.NewCell(string.Format("{0}:{1}", d.DaySensitivity ?? 0, d.NightSensitivity ?? 0)));
                                if (modeSensVolRows > 0)
                                {
                                    vsmText.Append("\n");
                                    dnText.Append("\n");
                                }
                                vsmText.Append(Cultures.Resources.Sensitivity);
                                dnText.Append(string.Format("{0}:{1}", dev.DaySensitivity, dev.NightSensitivity ?? 0));
                                modeSensVolRows++;
                            }
                                
                            tableRow.Cells.Add(TableUtil.NewCell(vsmText.ToString()));
                            tableRow.Cells.Add(TableUtil.NewCell(dnText.ToString(), TextAlignment.Center));
                        }
                        else
                        {
                            tableRow.Cells.Add(TableUtil.NewCell("--"));
                            tableRow.Cells.Add(TableUtil.NewCell("--", TextAlignment.Center));
                            modeSensVolRows++;
                        }

                        if (DeviceTypes.CurrentProtocolIsXfpApollo)
                        {
                            //remote LED
                            tableRow.Cells.Add(TableUtil.NewCell(DeviceTypes.CanHaveAncillaryBaseSounder(dev.DeviceType, DeviceTypes.CurrentProtocolType) ? dev.RemoteLEDEnabled ?? false ? "Y" : "N" : "--", TextAlignment.Center));

                            //base sounder group
                            tableRow.Cells.Add(TableUtil.NewCell((dev.RemoteLEDEnabled ?? false) || dev.AncillaryBaseSounderGroup is null ? "--" : string.Format(Cultures.Resources.Group_x, dev.AncillaryBaseSounderGroup.Value)));
                        }

                        // I/O config
                        int ioRowsUsed = 0;
                        if (dev.IsIODevice)
                        {
                            List<string> subaddressNames = DeviceTypes.CurrentProtocolIsXfpCast && dev.DeviceType == (int)XfpCastDeviceTypeIds.HS2
                                                            ? _xfpHushSubaddressNames
                                                            : _defaultSubaddressNames;

                            for (int io = 0; io < dev.IOConfig.Count; io++)
                            {
                                if (dev.IOConfig[io].InputOutput != IOTypes.NotUsed)
                                {
                                    if (ioRowsUsed > 0)
                                    {
                                        bodyGroup.Rows.Add(tableRow);
                                        tableRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground };
                                        for (int c = 0; c < _leftColumns; c++)
                                            tableRow.Cells.Add(TableUtil.NewCell(""));
                                    }

                                    var isGroup = dev.IOConfig[io].InputOutput == IOTypes.Output && dev.IsGroupedDevice;
                                    var isSet   = dev.IOConfig[io].InputOutput == IOTypes.Output && !dev.IsZonalDevice;

                                    if (dev.IOConfig[io].Index >= 0 && dev.IOConfig[io].Index < subaddressNames.Count)
                                        tableRow.Cells.Add(TableUtil.NewCell(subaddressNames[dev.IOConfig[io].Index]));
                                    else
                                        tableRow.Cells.Add(TableUtil.NewCell(""));

                                    tableRow.Cells.Add(TableUtil.NewCell(CTecDevices.Enums.IOTypeToString(dev.IOConfig[io].InputOutput)));
                                    tableRow.Cells.Add(TableUtil.NewCell(((dev.IOConfig[io].Channel ?? 0) + 1).ToString()));
                                    tableRow.Cells.Add(TableUtil.NewCell(zgsDescription(true, isGroup, isSet, (int)dev.IOConfig[io].ZoneGroupSet)));
                                    tableRow.Cells.Add(TableUtil.NewCell(GetDeviceName?.Invoke(dev.IOConfig[io].NameIndex)));

                                    ioRowsUsed++;
                                }
                            }
                        }

                        var rowSpan = 1 + ioRowsUsed;
                    }

                    bodyGroup.Rows.Add(tableRow);

                    if (++rowCount % 25 == 24)
                        break;
                }

                table.RowGroups.Add(bodyGroup);
                _doc.Blocks.Add(table);

                //GridUtil.AddRowToGrid(grid, 10);
            }
            catch (Exception ex) { }
            finally
            {
                PrintUtil.ResetFont();
            }

            //return new(grid);

            return (rowCount < _deviceSort.Count);
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
