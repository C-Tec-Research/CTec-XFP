using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class NetworkConfig : Page//, IXfpToolsPanelConfigPage
    {
        private const int _gridFaults = 0;
        private const int _gridAlarms = 1;
        private const int _gridControls = 2;
        private const int _gridDisables = 3;
        private const int _gridOccupied = 4;
        CheckBox[][] _chkboxGrid;


        public NetworkConfig()
        {
            InitializeComponent();
            DataContext = _context = new NetworkConfigViewModel(this);

            initGrid();
        }


        private NetworkConfigViewModel _context;


        private void initGrid()
        {
            _chkboxGrid = new CheckBox[][] { new CheckBox[] { chkFaultsPanel1, chkFaultsPanel2, chkFaultsPanel3, chkFaultsPanel4, chkFaultsPanel5, chkFaultsPanel6, chkFaultsPanel7, chkFaultsPanel8},
                                             new CheckBox[] { chkAlarmsPanel1, chkAlarmsPanel2, chkAlarmsPanel3, chkAlarmsPanel4, chkAlarmsPanel5, chkAlarmsPanel6, chkAlarmsPanel7, chkAlarmsPanel8},
                                             new CheckBox[] { chkControlsPanel1, chkControlsPanel2, chkControlsPanel3, chkControlsPanel4, chkControlsPanel5, chkControlsPanel6, chkControlsPanel7, chkControlsPanel8},
                                             new CheckBox[] { chkDisablesPanel1, chkDisablesPanel2, chkDisablesPanel3, chkDisablesPanel4, chkDisablesPanel5, chkDisablesPanel6, chkDisablesPanel7, chkDisablesPanel8},
                                             new CheckBox[] { chkOccupiedPanel1, chkOccupiedPanel2, chkOccupiedPanel3, chkOccupiedPanel4, chkOccupiedPanel5, chkOccupiedPanel6, chkOccupiedPanel7, chkOccupiedPanel8} };
        }

        public void SetPanel(int panel)
        {
            //lblPanel.Content = ((XfpFileViewModel)DataContext).Repeaters[panel].Name;
            foreach (CheckBox[] cc in _chkboxGrid)
                for (int i = 0; i < cc.Length; i++)
                    cc[i].IsEnabled = i != panel-1;
        }


        void SetCulture(CultureInfo culture)
        {
        }

        //void SetConfig(ToolsConfig config)
        //{
        //}


        //void SetProtocol(ProtocolTypes protocol)
        //{
        //    ((XfpFileViewModel)DataContext).SetProtocol(protocol);
        //}


        //public void SetDataContext(XfpFileViewModel context)
        //{
        //    DataContext = context;
        //}


        private void ctrl_PreviewMouseDown(object sender, MouseButtonEventArgs e)      { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e)                { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e) { if (e.Command == ApplicationCommands.Paste) if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }

        private void panelName1_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void panelName2_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void panelName3_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void panelName4_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void panelName5_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void panelName6_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void panelName7_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void panelName8_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }

        private void panelName1_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[0].Name = (sender as TextBox).Text;
        private void panelName2_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[1].Name = (sender as TextBox).Text;
        private void panelName3_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[2].Name = (sender as TextBox).Text;
        private void panelName4_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[3].Name = (sender as TextBox).Text;
        private void panelName5_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[4].Name = (sender as TextBox).Text;
        private void panelName6_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[5].Name = (sender as TextBox).Text;
        private void panelName7_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[6].Name = (sender as TextBox).Text;
        private void panelName8_TextChanged(object sender, TextChangedEventArgs e) => _context.Repeaters[7].Name = (sender as TextBox).Text;
    }
}
