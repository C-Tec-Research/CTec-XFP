using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CTecControls;
using CTecControls.Config;
using CTecControls.UI.DeviceSelector;
using CTecControls.ViewModels;
using Xfp.Cultures;
using Xfp.DataTypes.PanelData;
using CTecDevices.Protocol;
using Xfp.UI.Interfaces;
using Xfp.UI.ViewHelpers;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;
using CTecUtil;
using CTecControls.Util;
using Xfp.DataTypes;

namespace Xfp.Printing
{
    /// <summary>
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetails : Page
    {
        public DeviceDetails(XfpData data, int panelNumber)
        {
            InitializeComponent();
            DataContext = _context = new DeviceDetailsViewModel(this, null, grdDeviceSummaryLoop1, grdDeviceSummaryLoop2);
            
            _context.InitMenu = new((c) => { });
            _context.LoopChanged = new((l) => { });
         
            _context.SetCulture(CultureInfo.CurrentCulture);
            _context.PopulateView(data);
            _context.PanelNumber = panelNumber;
            grdDeviceSummaryLoop1.ItemsSource = _context.Loop1;
            grdDeviceSummaryLoop2.ItemsSource = _context.Loop2;
        }

        private DeviceDetailsViewModel _context;


        public DataGrid Loop1DetailsDataGrid => grdDeviceSummaryLoop1;
        public DataGrid Loop2DetailsDataGrid => grdDeviceSummaryLoop2;
    }
}
