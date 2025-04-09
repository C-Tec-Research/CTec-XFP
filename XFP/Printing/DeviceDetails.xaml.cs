using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xfp.DataTypes;

namespace Xfp.Printing
{
    /// <summary>
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetails : Page
    {
        public DeviceDetails(XfpData data, int panelNumber, CTecUtil.SortOrder sortOrder, bool showOnlyFittedDevices)
        {
            InitializeComponent();
            DataContext = _context = new DevicesPrintViewModel(this, data, panelNumber, sortOrder, showOnlyFittedDevices);
            
//            _context.InitMenu = new((c) => { });
//            _context.LoopChanged = new((l) => { });
         
//            _context.SetCulture(CultureInfo.CurrentCulture);
//            _context.PopulateView(data);
//            _context.PanelNumber = panelNumber;
            grdDeviceSummaryLoop1.ItemsSource = _context.Loop1;
            grdDeviceSummaryLoop2.ItemsSource = _context.Loop2;
        }

        private DevicesPrintViewModel _context;


        public DataGrid Loop1DetailsDataGrid => grdDeviceSummaryLoop1;
        public DataGrid Loop2DetailsDataGrid => grdDeviceSummaryLoop2;
    }
}
