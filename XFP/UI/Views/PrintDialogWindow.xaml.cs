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
using Newtonsoft.Json.Linq;
using CTecUtil.Config;
using CTecUtil.UI.Util;
using Xfp.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Composition;
using Xfp.DataTypes;
using System.Printing;

namespace Xfp.UI.Views
{
    public partial class PrintDialogWindow : Window
    {
        /// <summary>
        /// Instance of the Comms Log display window
        /// </summary>
        /// <param name="logData"></param>
        public PrintDialogWindow(ApplicationConfig applicationConfig, ObservableCollection<Page> pages, Page currentPage)
        {
            InitializeComponent();

            DataContext = _context = new PrintDialogWindowViewModel(applicationConfig, pages, currentPage);

            var owner = Application.Current.MainWindow;
            this.Left = owner.Left + owner.ActualWidth  / 2 - 100;
            this.Top  = owner.Top  + owner.ActualHeight / 2 - 200;
        }


//        public CTecUtil.PrinterSettings PrinterSettings => _context.PrinterSettings;
        public PrintingParameters PrintParams => _context.PrintParams;


        private PrintDialogWindowViewModel _context;
        private bool _isOpen;


        private void setSize()
        {
            grdOuter.Width  = grdInner.ActualWidth;
            grdOuter.Height = grdInner.ActualHeight;
            this.Width  = grdInner.ActualWidth;
            this.Height = grdInner.ActualHeight;
        }


        private void updateWindowParams(bool save = false) { if (_isOpen) _context.UpdateWindowParams(this, save); }


        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { updateWindowParams(); _context.Close(this); }


        //private void btnMinimise_Click(object sender, RoutedEventArgs e) { WindowState = WindowState.Minimized; updateWindowParams(); }
        //private void btnMaximise_Click(object sender, RoutedEventArgs e) { WindowState = WindowState.Maximized; updateWindowParams(); }
        //private void btnRestore_Click(object sender, RoutedEventArgs e)  { WindowState = WindowState.Normal;    updateWindowParams(); }
        //private void btnExit_Click(object sender, RoutedEventArgs e)           => Close();
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)  { setSize(); updateWindowParams(true); }
        //private void window_LocationChanged(object sender, EventArgs e)        => updateWindowParams(true);

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape: 
                    Close();
                    break;
            }
        }

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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isOpen = true;
        }

        /*
        FlowDocument: A FlowDocument is created with some sample text.
        PrintDialog: A PrintDialog is used to allow the user to select a printer and configure print settings.
        Print Action: If the user confirms the print action, the document is sent to the printer.
        - The FlowDocument can be customised with more complex content as needed.
        */
        private void PrintButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;


        private void ClosePrint_Click(object sender, EventArgs e)        => Close();
        //private void CancelPrint_Click(object sender, RoutedEventArgs e) => Close();


        private void cboPrinter_PreviewMouseDown(object sender, MouseButtonEventArgs e)   => _context.PrinterListIsOpen = true;
        private void PrintOptions_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.PrinterListIsOpen = false;
        private void PrinterList_MouseUp(object sender, MouseButtonEventArgs e)           => _context.PrinterListIsOpen = false;
        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e)   { try { DragMove(); updateWindowParams(); } catch { } }
    }
}
