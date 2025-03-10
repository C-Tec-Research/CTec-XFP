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
using CTecUtil.Config;
using CTecUtil.UI.Util;
using Newtonsoft.Json.Linq;
using Xfp.Config;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools.ValidationWindow;

namespace Xfp.UI.Views.PanelTools
{
    public partial class ValidationWindow : Window
    {
        /// <summary>
        /// Instance of the errors/warnings display window
        /// </summary>
        /// <param name="data"></param>
        /// <param name="currentPage">The currently-open app page.<br/>
        ///                           If this is not null the error tree will expand this page if there are any errors or warnings within.</param>
        public ValidationWindow(XfpData data, int currentPanel, int currentLoop, string currentPage)
        {
            InitializeComponent();

            DataContext = _context = new ValidationWindowViewModel(data, currentPanel, currentLoop, currentPage);
            
            restoreWindowState();
        }


        private ValidationWindowViewModel _context;


        public void Show(Window parent)
        {
            this.Top  = parent.Top  + parent.Height / 2 - this.Height / 2;
            this.Left = parent.Left + parent.Width  / 2 - this.Width  / 2;
            base.Show();
        }


        public void RefreshView()
        {
            tvwErrors.Items.Refresh();
            tvwErrors.UpdateLayout();
            tvwSiteErrors.Items.Refresh();
            tvwSiteErrors.UpdateLayout();
        }


        private void window_StateChanged(object sender, EventArgs e) { _context.ChangeWindowState(WindowState); updateWindowParams(); }
        private void updateWindowParams(bool save = false)           { XfpApplicationConfig.Settings.UpdateMonitorWindowParams(this, _context.LayoutTransform.ScaleX, save); }
        private void restoreWindowState()                           => _context.ChangeWindowState(this.WindowState = WindowUtil.SetWindowDimensions(this, XfpApplicationConfig.Settings.ValidationWindow));


        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XfpApplicationConfig.Settings.UpdateValidationWindowParams(this, _context.LayoutTransform.ScaleX, true);
            _context.Close();
        }


        /// <summary>
        /// For some reason this ScrollViewer doesn't scroll, so handle the mouswheel explicitly here.
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ContentVerticalOffset - e.Delta / 4);
            e.Handled = true;
        }

        private void btnMinimise_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void btnMaximise_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void btnRestore_Click(object sender, RoutedEventArgs e)  => WindowState = WindowState.Normal;
        private void btnExit_Click(object sender, RoutedEventArgs e)     => Close();

        private void window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                //Ctrl+mouse wheel zooms the screen
                if (e.Delta > 0)
                    _context.ZoomIn();
                else
                    _context.ZoomOut();
            }
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
                Close();
        }
    }
}
