using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Shapes;
using CTecControls.Util;
using CTecUtil;
using CTecUtil.UI;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Xfp.UI.Views
{
    /// <summary>
    /// Interaction logic for PanelManagementWindow.xaml
    /// </summary>
    public partial class PanelManagementWindow : Window
    {
        public PanelManagementWindow(XfpData data, CTecDevices.ObjectTypes protocol, List<int> panelNumberSet)
        {
            InitializeComponent();

            DataContext = _context = new PanelManagementWindowViewModel(data, protocol, panelNumberSet, this);
        }


        private PanelManagementWindowViewModel _context;


        public void ShowDialog(Window owner)
        {
            this.Top = owner.Top + 35;
            this.Left = owner.Left + 360;
            base.ShowDialog();
        }


        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e) { try { DragMove(); /*updateWindowParams();*/ } catch { } }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width  = grdPanelManagement.ActualWidth + 54;
            this.Height = grdPanelManagement.ActualHeight + btnSave.ActualHeight + 90;
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => _context.Close();


        private void closePanelManagementPopup_Click(object sender, EventArgs e)        { if (_context.ClosePanelManagementPopup(false)) this.Close(); }
        private void savePanelManagementPopup_Click(object sender, RoutedEventArgs e)   { if (_context.ClosePanelManagementPopup(true)) this.Close(); }
        private void cancelPanelManagementPopup_Click(object sender, RoutedEventArgs e) { if (_context.ClosePanelManagementPopup(false)) this.Close(); }


        private void AddPanel_Click(object sender, RoutedEventArgs e)
        {
            if (UITools.FindDataGridRow((DependencyObject)e.OriginalSource, 2)?.DataContext is PanelManagementItemViewModel panel)
                _context.AddPanel(panel);
        }

        private void RemovePanel_Click(object sender, RoutedEventArgs e)
        {
            if (UITools.FindDataGridRow((DependencyObject)e.OriginalSource, 3)?.DataContext is PanelManagementItemViewModel panel)
                _context.RemovePanel(panel);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                closePanelManagementPopup_Click(sender, e);
        }
    }
}
