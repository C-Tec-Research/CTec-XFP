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

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceConfigData
    {
        public void Print(FlowDocument doc, int panelNumber, bool printAllLoopDevices)
        {
            var devicePage = new Section();
            devicePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1)));

            devicePage.Blocks.Add(deviceList(printAllLoopDevices));

            doc.Blocks.Add(devicePage);
        }


        internal static DeviceNameGetter GetDeviceName;


        private int _totalColumns = 14;
        private int _ioSettingsColumns = 5;
        private static SolidColorBrush _seeIoSettingsForeground = (SolidColorBrush)Application.Current.FindResource("BrushWarn");
        private static SolidColorBrush _ioSubheaderBrush        = (SolidColorBrush)Application.Current.FindResource("Brush04");


        public BlockUIContainer deviceList(bool printAllLoopDevices)
        {
            var grid = columnHeaders();

            int row = grid.RowDefinitions.Count;
            int col;
            int dataRows = 0;

            foreach (var d in Devices)
            {
                dataRows++;
                col = 0;

                if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                    grid.Children.Add(PrintUtil.GridCell(d.Index + 1, row, col++, HorizontalAlignment.Right));
                }

                if (DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    grid.Children.Add(PrintUtil.GridCellImage(DeviceTypes.DeviceIcon(d.DeviceType, DeviceTypes.CurrentProtocolType), row, col++, 18, 18));
                    grid.Children.Add(PrintUtil.GridCell(DeviceTypes.DeviceTypeName(d.DeviceType, DeviceTypes.CurrentProtocolType), row, col++));

                    if (d.IsIODevice)
                    {
                        grid.Children.Add(PrintUtil.SetForeground(PrintUtil.GridCell(Cultures.Resources.See_IO_Configuration, row, col++, HorizontalAlignment.Left, PrintUtil.PrintDefaultFontSize, FontStyles.Italic), _seeIoSettingsForeground));
                    }
                    else
                    {
                        grid.Children.Add(PrintUtil.GridCell(zgsDescription(false, d.IsGroupedDevice, false, d.IsGroupedDevice ? d.Group : d.Zone), row, col++));
                    }

                    grid.Children.Add(PrintUtil.GridCell(GetDeviceName?.Invoke(d.NameIndex), row, col++));
                    grid.Children.Add(PrintUtil.GridCell(d.IsVolumeDevice ? Cultures.Resources.Volume : Cultures.Resources.Sensitivity, row, col++));
                    grid.Children.Add(PrintUtil.GridCell(string.Format("{0}:{1}", d.DaySensitivity ?? 0, d.NightSensitivity ?? 0), row, col++, HorizontalAlignment.Center));

                    if (DeviceTypes.CurrentProtocolIsXfpApollo)
                    {
                        grid.Children.Add(PrintUtil.GridCellBool(d.RemoteLEDEnabled ?? false, row, col++, false, false));
                        grid.Children.Add(PrintUtil.GridCell(d.AncillaryBaseSounderGroup is null ? "--" : d.AncillaryBaseSounderGroup.Value.ToString(), row, col++));
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
                                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                    grid.Children.Add(PrintUtil.GridBackground(ioRow, 0, 1, _totalColumns, Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                                    newRows++;
                                }

                                var isGroup = d.IOConfig[i].InputOutput == CTecDevices.IOTypes.Output && d.IsGroupedDevice;
                                var isSet   = d.IOConfig[i].InputOutput == CTecDevices.IOTypes.Output && !d.IsZonalDevice;

                                grid.Children.Add(PrintUtil.GridCell(d.IOConfig[i].Index + 1,                                               ioRow, ioCol++));
                                grid.Children.Add(PrintUtil.GridCell(CTecDevices.Enums.IOTypeToString(d.IOConfig[i].InputOutput),           ioRow, ioCol++));
                                grid.Children.Add(PrintUtil.GridCell((d.IOConfig[i].Channel ?? 0) + 1,                                      ioRow, ioCol++));
                                grid.Children.Add(PrintUtil.GridCell(zgsDescription(true, isGroup, isSet, (int)d.IOConfig[i].ZoneGroupSet), ioRow, ioCol++));
                                grid.Children.Add(PrintUtil.GridCell(GetDeviceName?.Invoke(d.IOConfig[i].NameIndex),                        ioRow, ioCol++));
                                ioRow++;
                            }
                        }

                        row += newRows;
                    }
                }
                    
                row++;
            }

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < _totalColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridBackground(0, 0, 2, _totalColumns, PrintUtil.GridHeaderBackground));

            int col = 0;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Number_Symbol,           0, col++, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Device_Type,             0, col, 1, 2));
            col += 2;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone_Group,              0, col++));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Device_Name,             0, col++));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Volume_Sensitivity_mode, 0, col++, 2, 1));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Day_Night,               0, col++));

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Remote_LED_Header,   0, col++));
                grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Base_Sounder_Header, 0, col++));
            }

            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.IO_Configuration,        0, col, 1, _ioSettingsColumns));
            
            grid.Children.Add(PrintUtil.SetForeground(PrintUtil.GridCell(Cultures.Resources.I_O,            1, col++, 1, 2, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            col++;
            grid.Children.Add(PrintUtil.SetForeground(PrintUtil.GridCell(Cultures.Resources.Channel_Abbr,   1, col++, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(PrintUtil.SetForeground(PrintUtil.GridCell(Cultures.Resources.Zone_Group_Set, 1, col++, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));
            grid.Children.Add(PrintUtil.SetForeground(PrintUtil.GridCell(Cultures.Resources.Description,    1, col++, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom), _ioSubheaderBrush));

            return grid;
        }
        

        private string zgsDescription(bool isIODevice, bool isGroupedDevice, bool isSetDevice, int value)
        {
            if (value == 0)
                return Cultures.Resources.Use_In_Special_C_And_E;

            if (isGroupedDevice)
                return string.Format(Cultures.Resources.Group_x, value);

            if (!isIODevice && isSetDevice) 
                return string.Format(Cultures.Resources.Set_x, value);

            return string.Format(Cultures.Resources.Zone_x, value);
        }
    }
}
