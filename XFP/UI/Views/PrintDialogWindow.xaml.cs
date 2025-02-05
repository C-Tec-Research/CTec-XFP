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

namespace Xfp.UI.Views
{
    public partial class PrintDialogWindow : Window
    {
        /// <summary>
        /// Instance of the Comms Log display window
        /// </summary>
        /// <param name="logData"></param>
        public PrintDialogWindow(ApplicationConfig applicationConfig)
        {
            InitializeComponent();

            DataContext = _context = new PrintDialogWindowViewModel(applicationConfig);
            //_context.AppendText = appendText;
            //_context.ReadText(debugMode);

            restoreWindowState();
        }


        private PrintDialogWindowViewModel _context;


        public void Show(Window parent)
        {
            this.Top = parent.Top + parent.Height / 2 - this.Height / 2;
            this.Left = parent.Left + parent.Width / 2 - this.Width / 2;
            base.Show();
        }


//        private void appendText(string text, Color color)
//        {
//            //rtb.SuspendLayout();
//            //rtb. = rtb.Text.Length;
//            rtb.SelectionTextBrush = new SolidColorBrush(color);
////            rtb.SelectionBrush = new SolidColorBrush(color);
//            var w = CTecUtil.TextProcessing.MeasureText(text, rtb.FontFamily, rtb.FontSize, rtb.FontStyle, rtb.FontWeight, rtb.FontStretch).Width + 8;
//            if (rtb.Width < w)
//                rtb.Width = w;
//            rtb.AppendText(text + "\r");
//           // rtb.SelectionLength = text.Length;
//            rtb.ScrollToEnd();
//            //box.ResumeLayout();
//        }


        public void ScrollToEnd() => scrollViewer.ScrollToEnd();
        public void RefreshView() { }


        private void window_StateChanged(object sender, EventArgs e) { _context.ChangeWindowState(WindowState); updateWindowParams(); }
        
        private void restoreWindowState()                  => _context.ChangeWindowState(this.WindowState = WindowUtil.SetWindowDimensions(this, _context.ApplicationConfig.LogWindow));
        private void updateWindowParams(bool save = false) => _context.UpdateWindowParams(this, save);


        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => _context.Close(this);


        /// <summary>
        /// For some reason this ScrollViewer doesn't scroll, so handle the mouswheel explicitly here.
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ContentVerticalOffset - e.Delta / 4);
            e.Handled = true;
        }

        private void btnMinimise_Click(object sender, RoutedEventArgs e) { WindowState = WindowState.Minimized; updateWindowParams(); }
        private void btnMaximise_Click(object sender, RoutedEventArgs e) { WindowState = WindowState.Maximized; updateWindowParams(); }
        private void btnRestore_Click(object sender, RoutedEventArgs e)  { WindowState = WindowState.Normal;    updateWindowParams(); }
        private void btnExit_Click(object sender, RoutedEventArgs e)           => Close();
        private void window_SizeChanged(object sender, SizeChangedEventArgs e) { /*rtb.Height = rtbContainer.Width;*/ updateWindowParams(true); }
        private void window_LocationChanged(object sender, EventArgs e)        => updateWindowParams(true);

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Home: 
                    scrollViewer.ScrollToHome();
                    break;
                case Key.End: 
                    scrollViewer.ScrollToEnd();
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


        private void Window_Loaded(object sender, RoutedEventArgs e) { }// => rtb.Width = rtbContainer.Width;

        /*
        FlowDocument: A FlowDocument is created with some sample text.
        PrintDialog: A PrintDialog is used to allow the user to select a printer and configure print settings.
        Print Action: If the user confirms the print action, the document is sent to the printer.
        - This example provides a basic framework for printing a document in a WPF application. You can customize the FlowDocument with more complex content as needed.
        */
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a FlowDocument
            FlowDocument doc = new FlowDocument(new Paragraph(new Run("Hello, this is a test document for printing.")));

            // Create a PrintDialog
            PrintDialog printDialog = new PrintDialog();

            // Check if the user clicked the Print button
            if (printDialog.ShowDialog() == true)
            {
                // Print the document
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing FlowDocument");
            }
        }

        private void ClosePrint_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cboPrinter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CancelPrint_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PrinterList_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
