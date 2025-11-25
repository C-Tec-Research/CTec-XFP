using CTecControls.UI;
using CTecDevices;
using CTecDevices.DataTypes;
using CTecDevices.DeviceTypes;
using CTecDevices.Protocol;
using CTecUtil;
using CTecUtil.Printing;
using CTecUtil.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using static Xfp.ViewModels.PanelTools.DeviceItemViewModel;

namespace Xfp.DataTypes.PanelData
{
    public partial class LoopConfigData
    {
        public void GetReport(FlowDocument doc, int panelNumber, bool printLoop1, bool printLoop2, bool printAllLoopDevices, SortOrder printOrder, CTecUtil.PrintActions printAction)
        {
            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x, panelNumber) + " - " + Cultures.Resources.Nav_Device_Details);

            GridUtil.ResetDefaults();
            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
            TableUtil.SetFontStretch(PrintUtil.FontNarrowWidth);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(PrintUtil.DefaultTableMargin);

            if (printLoop1) doc.Blocks.Add(printDeviceList(0, printAllLoopDevices, printOrder));
            if (printLoop2) doc.Blocks.Add(printDeviceList(1, printAllLoopDevices, printOrder));

            TableUtil.ResetDefaults();
        }


        internal static DeviceNamesEntryGetter GetDeviceName;

        private int       _totalColumns;
        private int       _leftColumns;
        private int       _ioSettingsColumns = 5;

        private static SolidColorBrush _seeIoSettingsForeground = Styles.SeeDetailsPrintBrush;
        
        private readonly List<string> _defaultSubaddressNames = new() { "0", "1", "2", "3" };
        private readonly List<string> _xfpHushSubaddressNames = new() { Cultures.Resources.Subaddress_Hush_0, 
                                                                        Cultures.Resources.Subaddress_Hush_1, 
                                                                        Cultures.Resources.Subaddress_Hush_2, 
                                                                        Cultures.Resources.Subaddress_Hush_3 };


        private Table printDeviceList(int loopNum, bool printAllLoopDevices, SortOrder printOrder)
        {
            int dataRows = 0;
            
            var reportName = string.Format(Cultures.Resources.Loop_x_Devices, loopNum + 1);

            try
            {
                var table = TableUtil.NewTable(reportName);

                defineColumnHeaders(table, reportName);

                var bodyGroup = new TableRowGroup();

                foreach (var dev in sortDevicesForPrinting(loopNum, printOrder, printAllLoopDevices))
                {
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
                    var mvsRows = (dev.IsModeDevice ? 1 : 0) + (dev.IsVolumeDevice ? 1 : 0) + (dev.IsSensitivityDevice ? 1 : 0);
                    var numRows = Math.Max(Math.Max(ioRows, vsmRows), mvsRows);

                    //create the required number of table rows
                    var newRows = new List<TableRow>();
                    for (int r = 0; r < numRows; r++)
                        newRows.Add(new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.NoBackground });
                    foreach (var row in newRows)
                        bodyGroup.Rows.Add(row);

                    //device number
                    var colNum = TableUtil.NewCell((dev.Index + 1).ToString(), numRows, 1, TextAlignment.Right);
                    colNum.Padding = new(0,0,5,0);
                    newRows[0].Cells.Add(colNum);


                    if (DeviceTypes.IsValidDeviceType(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                    {
                        //device icon
                        newRows[0].Cells.Add(TableUtil.NewCellImage(getDeviceIcon(dev.DeviceType), numRows, 1));

                        //device type name
                        newRows[0].Cells.Add(TableUtil.NewCell(DeviceTypes.DeviceTypeName(dev.DeviceType, DeviceTypes.CurrentProtocolType), numRows, 1));

                        //zone/group/set
                        if (dev.IsIODevice)
                            newRows[0].Cells.Add(TableUtil.NewCell("  " + Cultures.Resources.See_IO_Configuration_Abbr, numRows, 1, _seeIoSettingsForeground, FontStyles.Italic));
                        else
                            newRows[0].Cells.Add(TableUtil.NewCell(zgsDescription(dev), numRows, 1, true));

                        //name
                        newRows[0].Cells.Add(TableUtil.NewCell(GetDeviceName?.Invoke(dev.NameIndex), numRows, 1));


                        //volume/sensitivity/mode & day:night values
                        int modeSensVolRows = 0;
                        bool modeSensVolIsValid = true;

                        if (dev.IsModeDevice || dev.IsVolumeDevice || dev.IsSensitivityDevice)
                        {
                            if (dev.IsModeDevice)
                            {
                                newRows[modeSensVolRows].Cells.Add(TableUtil.NewCell(Cultures.Resources.Mode));

                                var validDM = isValidMode(dev.DayMode, dev.DeviceType);
                                var validNM = isValidMode(dev.NightMode, dev.DeviceType);
                                var dMode = validDM ? dev.DayMode.ToString()   : PrintUtil.ErrorValue;
                                var nMode = validNM ? dev.NightMode.ToString() : PrintUtil.ErrorValue;

                                var mCell = TableUtil.NewCell(string.Format("{0}:{1}", dMode, nMode));
                                if (!validDM || !validNM)
                                {
                                    mCell.Foreground = PrintUtil.ErrorBrush;
                                    mCell.FontStyle = FontStyles.Italic;
                                }
                                newRows[modeSensVolRows].Cells.Add(mCell);
                                modeSensVolRows++;
                            }

                            if (dev.IsVolumeDevice)
                            {
                                newRows[modeSensVolRows].Cells.Add(TableUtil.NewCell(Cultures.Resources.Volume));

                                var validDV = isValidVolume(dev.DayVolume);
                                var validNV = isValidVolume(dev.NightVolume);
                                var dVol = validDV ? (dev.DayVolume + 1).ToString()   : PrintUtil.ErrorValue;
                                var nVol = validNV ? (dev.NightVolume + 1).ToString() : PrintUtil.ErrorValue;

                                var vCell = TableUtil.NewCell(string.Format("{0}:{1}", dVol, nVol));
                                if (!validDV || !validNV)
                                {
                                    vCell.Foreground = PrintUtil.ErrorBrush;
                                    vCell.FontStyle = FontStyles.Italic;
                                }
                                newRows[modeSensVolRows].Cells.Add(vCell);
                                modeSensVolRows++;
                            }

                            if (dev.IsSensitivityDevice)
                            {
                                newRows[modeSensVolRows].Cells.Add(TableUtil.NewCell(Cultures.Resources.Sensitivity));

                                var validDS = isValidSensitivity(dev.DaySensitivity, dev.IsSensitivityHighDevice);
                                var validNS = isValidSensitivity(dev.NightSensitivity, dev.IsSensitivityHighDevice);
                                var dSens = validDS ? dev.DaySensitivity.ToString()   : PrintUtil.ErrorValue;
                                var nSens = validNS ? dev.NightSensitivity.ToString() : PrintUtil.ErrorValue;

                                var vCell = TableUtil.NewCell(string.Format("{0}:{1}", dSens, nSens));
                                if (!validDS || !validNS)
                                {
                                    vCell.Foreground = PrintUtil.ErrorBrush;
                                    vCell.FontStyle = FontStyles.Italic;
                                }
                                newRows[modeSensVolRows].Cells.Add(vCell);
                            }
                        }
                        else
                        {
                            newRows[0].Cells.Add(TableUtil.NewCell("--", numRows, 1));
                            newRows[0].Cells.Add(TableUtil.NewCell("--", numRows, 1, TextAlignment.Center));
                            modeSensVolRows++;
                        }

                        if (DeviceTypes.CurrentProtocolIsXfpApollo)
                        {
                            //remote LED
                            newRows[0].Cells.Add(TableUtil.NewCell(DeviceTypes.CanHaveAncillaryBaseSounder(dev.DeviceType, DeviceTypes.CurrentProtocolType) ? dev.RemoteLEDEnabled ?? false ? "Y" : "N" : "--", numRows, 1, TextAlignment.Center));

                            //base sounder group
                            newRows[0].Cells.Add(TableUtil.NewCell((dev.RemoteLEDEnabled ?? false) || dev.AncillaryBaseSounderGroup is null ? "--" : string.Format(Cultures.Resources.Group_x, dev.AncillaryBaseSounderGroup.Value), numRows, 1));
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
                                    var isGroup = dev.IOConfig[io].InputOutput == IOTypes.Output && dev.IsGroupedDevice;
                                    var isSet   = dev.IOConfig[io].InputOutput == IOTypes.Output && !dev.IsZonalDevice;

                                    if (dev.IOConfig[io].Index >= 0 && dev.IOConfig[io].Index < subaddressNames.Count)
                                        newRows[io].Cells.Add(TableUtil.NewCell(subaddressNames[dev.IOConfig[io].Index]));
                                    else
                                        newRows[io].Cells.Add(TableUtil.NewCell(""));

                                    newRows[io].Cells.Add(TableUtil.NewCell(CTecDevices.Enums.IOTypeToString(dev.IOConfig[io].InputOutput)));
                                    newRows[io].Cells.Add(TableUtil.NewCell(((dev.IOConfig[io].Channel ?? 0) + 1).ToString()));
                                    newRows[io].Cells.Add(TableUtil.NewCell(zgsDescription(dev, true, io), true));
                                    newRows[io].Cells.Add(TableUtil.NewCell(GetDeviceName?.Invoke(dev.IOConfig[io].NameIndex)));

                                    ioRowsUsed++;
                                }
                            }
                        }

                        var rowSpan = 1 + ioRowsUsed;
                    }
                }

                table.RowGroups.Add(bodyGroup);
                return table;
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(string.Format(CTecUtil.Cultures.Resources.Error_Generating_Report_x, reportName), CTecUtil.Cultures.Resources.Error_Printing, ex);
                return null;
            }
            finally
            {
                PrintUtil.ResetFont();
            }
        }


        private bool isValidMode(int? mode, int? deviceType)
        {
            var validModes = getValidModes(deviceType);
            return mode.HasValue && validModes.Count > 0 && mode >= validModes[0] && mode <= validModes[^1];
        }

        private List<int> getValidModes(int? deviceType) => (from m in DeviceTypes.ModeSettings(deviceType) select m.Index).ToList();

        private bool isValidVolume(int? vol) => vol.HasValue && vol >= DeviceConfigData.MinVolume && vol <= DeviceConfigData.MaxVolume;

        private bool isValidSensitivity(int? sens, bool isSensHighDevice)
            => sens.HasValue && isSensHighDevice ? sens >= DeviceConfigData.MinSensitivityHigh && sens <= DeviceConfigData.MaxSensitivityHigh
                                                 : sens >= DeviceConfigData.MinSensitivity && sens <= DeviceConfigData.MaxSensitivity;


        private void defineColumnHeaders(Table table, string reportHeader)
        {
            TableUtil.SetFontSize(PrintUtil.PrintPageHeaderFontSize);

            //measure required column widths for subaddress and IO headers
            var cellMargins      = (int)(PrintUtil.DefaultTableMargin.Left + PrintUtil.DefaultTableMargin.Right) + 1;
            
            var subaddressHeader = Cultures.Resources.Subaddress_Short;
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
            var wZSG  = 70;
            var wName = 85;
            var wChan = TableUtil.MeasureText(Cultures.Resources.Channel_Abbr).Width + 1;
            var wDN   = TableUtil.MeasureText(Cultures.Resources.Day_Night).Width + 1;
            
            TableUtil.SetFontSize(PrintUtil.PrintDefaultFontSize);

            //define table's columns
            _totalColumns = 0;
            table.Columns.Add(new TableColumn() { Width = new GridLength(18) });              _totalColumns++;    // num
            table.Columns.Add(new TableColumn() { Width = new GridLength(30) });              _totalColumns++;    // icon
            table.Columns.Add(new TableColumn() { Width = new GridLength(wType) });           _totalColumns++;    // type name
            table.Columns.Add(new TableColumn() { Width = new GridLength(wZSG) });            _totalColumns++;    // z/g/s
            table.Columns.Add(new TableColumn() { Width = new GridLength(wName) });           _totalColumns++;    // name
            table.Columns.Add(new TableColumn() { Width = new GridLength(46) });              _totalColumns++;    // v/s/m
            table.Columns.Add(new TableColumn() { Width = new GridLength(wDN) });             _totalColumns++;    // day:night

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                table.Columns.Add(new TableColumn() { Width = new GridLength(50) });          _totalColumns++;    // remote LED
                table.Columns.Add(new TableColumn() { Width = new GridLength(50) });          _totalColumns++;    // base sounder grp
            }
            _leftColumns = _totalColumns;

            table.Columns.Add(new TableColumn() { Width = new GridLength(subaddressWidth) }); _totalColumns++;    // subaddress
            table.Columns.Add(new TableColumn() { Width = new GridLength(ioWidth) });         _totalColumns++;    // i/o
            table.Columns.Add(new TableColumn() { Width = new GridLength(wChan) });           _totalColumns++;    // chan
            table.Columns.Add(new TableColumn() { Width = new GridLength(wZSG) });            _totalColumns++;    // z/g/s
            table.Columns.Add(new TableColumn() { Width = new GridLength(wName) });           _totalColumns++;    // name
            
            //define rows for the header
            var headerRow1 = new TableRow();
            var headerRow2 = new TableRow();
            var headerRow3 = new TableRow();

            headerRow1.Background = headerRow2.Background = headerRow3.Background = PrintUtil.TableHeaderBackground;
            
            var colHeader = TableUtil.NewCell(reportHeader, 1, _totalColumns, FontWeights.Bold);
            colHeader.FontSize = PrintUtil.PrintDefaultFontSize;
            colHeader.Padding = new(5,0,0,0);
            headerRow1.Cells.Add(colHeader);

            var colNum = TableUtil.NewCell(Cultures.Resources.Number_Symbol, 3, 1, TextAlignment.Right, FontWeights.Bold);
            colNum.Padding = new(0,0,5,0);
            headerRow2.Cells.Add(colNum);

            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Device_Type,             2, 2, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Zone_Group,              2, 1, FontWeights.Bold));
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
            headerRow2.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.IO_Configuration, 1, _ioSettingsColumns, FontWeights.Bold), Styles.Brush06));

            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow1);
            headerGroup.Rows.Add(headerRow2);
            headerGroup.Rows.Add(headerRow3);
            table.RowGroups.Add(headerGroup);
        }
        

        public List<DeviceData> sortDevicesForPrinting(int loopNum, SortOrder printOrder, bool printAll)
        {
            var result = new List<DeviceData>(Loops[loopNum].Devices);

            if (!printAll)
                for (int i = result.Count - 1; i > 0; i--)
                    if (!DeviceTypes.IsValidDeviceType(result[i].DeviceType, DeviceTypes.CurrentProtocolType))
                        result.RemoveAt(i);

            if (printOrder == SortOrder.Type)
                result.Sort(compareByDeviceType);
            else if (printOrder == SortOrder.ZoneGroupSet)
                result.Sort(compareByZoneGroupSet);

            return result;
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
        

        private Image getDeviceIcon(int? deviceType)
            => new Image()
            {
                Source = DeviceTypes.DeviceIcon(deviceType, DeviceTypes.CurrentProtocolType), 
                Width = 14, Height = 14, 
                HorizontalAlignment = HorizontalAlignment.Left, 
                VerticalAlignment = VerticalAlignment.Center, 
                Margin = new(0) 
            };
        

        private string zgsDescription(DeviceData device, bool isIOSetting = false, int? ioIndex = null)
        {
            var isIODevice = device.IsIODevice && ioIndex.HasValue;
            var value      = isIODevice ? device.IOConfig[(int)ioIndex].ZoneGroupSet : device.Zone;
            
            if (value < 0 || value > ZoneConfigData.NumZones)
                return (null);

            if (value == 0)
                return Cultures.Resources.Use_In_Special_C_And_E;

            var isSet     = device.IsIODevice && device.IOConfig[(int)ioIndex].InputOutput == IOTypes.Output && !device.IsZonalDevice;
            var formatStr = device.IsGroupedDevice ? Cultures.Resources.Group_x : isSet ? Cultures.Resources.Set_x : Cultures.Resources.Zone_x;

            return string.Format(formatStr, value);
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
