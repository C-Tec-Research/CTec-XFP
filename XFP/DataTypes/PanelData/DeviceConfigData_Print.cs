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
            //_deviceIconStyle = (Style)Application.Current.FindResource("MainDataGridIcon");

            var devicePage = new Section();
            devicePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1)));

            devicePage.Blocks.Add(deviceList(printAllLoopDevices));

            doc.Blocks.Add(devicePage);
        }


        internal static DeviceNameGetter GetDeviceName;


        private int _totalColumns = 13;
        //private static Style _deviceIconStyle = (Style)Application.Current.FindResource("MainDataGridIcon");


        public BlockUIContainer deviceList(bool printAllLoopDevices)
        {
            var grid = columnHeaders();


            int gridRow = 1;
            int device = 0;

            foreach (var d in Devices)
            {
                if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    grid.Children.Add(PrintUtil.GridBackground(gridRow, 0, 1, _totalColumns, Int32.IsOddInteger(device) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                    grid.Children.Add(PrintUtil.GridCell(d.Index + 1, gridRow, 0, HorizontalAlignment.Right));

                    Image deviceIcon = new Image();
                    deviceIcon.Width = 18;
                    deviceIcon.Height = 18;
                    deviceIcon.Margin = new(2);
                    ImageSource image = DeviceTypes.DeviceIcon(d.DeviceType, DeviceTypes.CurrentProtocolType);
                    deviceIcon.Source = image;

                    Grid.SetRow(deviceIcon, gridRow);
                    Grid.SetColumn(deviceIcon, 1);
                    grid.Children.Add(deviceIcon);

                    grid.Children.Add(PrintUtil.GridCell(DeviceTypes.DeviceTypeName(d.DeviceType, DeviceTypes.CurrentProtocolType), gridRow, 2));
                    
                    int ioRows = 0;

                    if (d.IsIODevice)
                    {
                        //add I/O settings column headers
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.I_O,            gridRow, 3, 1, 2, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Channel_Abbr,   gridRow, 5, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Zone_Group_Set, gridRow, 6, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Description,    gridRow, 7, 1, 1, false, HorizontalAlignment.Left, 7, FontStyles.Italic, PrintUtil.PrintDefaultFont, VerticalAlignment.Bottom));
                        ioRows++;
                        
                        //add I/O settings values
                        for (int i = 0; i < d.IOConfig.Count; i++)
                        {
                            grid.Children.Add(PrintUtil.GridBackground(gridRow + ioRows, 0, 1, _totalColumns, Int32.IsOddInteger(device) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                            if (d.IOConfig[i].InputOutput != CTecDevices.IOTypes.NotUsed)
                            {
                                grid.Children.Add(PrintUtil.GridCell(d.IOConfig[i].Index + 1,                                           gridRow + ioRows, 3, HorizontalAlignment.Left, 6, FontStyles.Normal));
                                grid.Children.Add(PrintUtil.GridCell(CTecDevices.Enums.IOTypeToString(d.IOConfig[i].InputOutput),       gridRow + ioRows, 4, HorizontalAlignment.Left, 6, FontStyles.Normal));
                                grid.Children.Add(PrintUtil.GridCell((d.IOConfig[i].Channel ?? 0) + 1,                                  gridRow + ioRows, 5, HorizontalAlignment.Left, 6, FontStyles.Normal));

                                var isGroup = d.IOConfig[i].InputOutput == CTecDevices.IOTypes.Output && d.IsGroupedDevice;
                                var isSet   = d.IOConfig[i].InputOutput == CTecDevices.IOTypes.Output && !d.IsZonalDevice;
                                grid.Children.Add(PrintUtil.GridCell(zgsDescription(isGroup, isSet, (int)d.IOConfig[i].ZoneGroupSet),   gridRow + ioRows, 6, HorizontalAlignment.Left, 6, FontStyles.Normal));
                                
                                grid.Children.Add(PrintUtil.GridCell(GetDeviceName?.Invoke(d.IOConfig[i].NameIndex),                    gridRow + ioRows, 7, HorizontalAlignment.Left, 6, FontStyles.Normal));
                                ioRows++;
                            }
                        }
                    }
                    else
                    {
                        grid.Children.Add(PrintUtil.GridCell(zgsDescription(d.IsGroupedDevice, false, d.Zone),                          gridRow, 3, 1, 5));
                    }

                    grid.Children.Add(PrintUtil.GridCell(GetDeviceName?.Invoke(d.NameIndex),                                            gridRow, 8));
                    grid.Children.Add(PrintUtil.GridCell(d.IsVolumeDevice ? Cultures.Resources.Volume : Cultures.Resources.Sensitivity, gridRow, 9));
                    grid.Children.Add(PrintUtil.GridCell(string.Format("{0}:{1}", d.DaySensitivity ?? 0, d.NightSensitivity ?? 0),      gridRow, 10, HorizontalAlignment.Center));

                    if (DeviceTypes.CurrentProtocolIsXfpApollo)
                    {
                        grid.Children.Add(PrintUtil.GridCellBool(d.RemoteLEDEnabled ?? false,                                                           gridRow, 11, false, false));
                        grid.Children.Add(PrintUtil.GridCell(d.AncillaryBaseSounderGroup is null ? "--" : d.AncillaryBaseSounderGroup.Value.ToString(), gridRow, 12));
                    }
                    
                    gridRow += ioRows + 1;

                    device++;
                }
            }

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < _totalColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridBackground(0, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));

            int col = 0;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Number_Symbol,           0, col++, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Device_Type,             0, col++, 1, 2));
            col++;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone_Group,              0, col++, 1, 5));
            col += 4;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Device_Name,             0, col++));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Volume_Sensitivity_mode, 0, col++));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Day_Night,               0, col++));

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Remote_LED_Header,   0, col++));
                grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Base_Sounder_Header, 0, col++));
            }

            return grid;
        }
        

        private string zgsDescription(bool isGroupedDevice, bool isSetDevice, int value)
        {
            if (isGroupedDevice)
                return valueOrUseInSpecialCAndE(Cultures.Resources.Group_x, value);
            else if (isSetDevice) 
                return valueOrUseInSpecialCAndE(Cultures.Resources.Set_x, value);
            return valueOrUseInSpecialCAndE(Cultures.Resources.Zone_x, value);
        }

        private string valueOrUseInSpecialCAndE(string textIfNotSpecial, int value) => string.Format(value > 0 ? textIfNotSpecial : Cultures.Resources.Use_In_Special_C_And_E, value);
    }
}
