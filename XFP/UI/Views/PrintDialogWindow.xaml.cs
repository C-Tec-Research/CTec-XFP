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
using Windows.Media.Capture.Core;
using CTecControls.UI;
using CTecUtil;
using System.Globalization;

namespace Xfp.UI.Views
{
    public partial class PrintDialogWindow : Window
    {
        public PrintDialogWindow(ApplicationConfig applicationConfig, ObservableCollection<Page> pages, Page currentPage)
        {
            InitializeComponent();

            DataContext = _context = new PrintDialogWindowViewModel(applicationConfig, pages, currentPage, btnPrint, btnPreview);
            
            addShortcutKey(btnPrint);
            addShortcutKey(btnPreview);
            _context.CloseAction = printOrPreview;

            var owner = Application.Current.MainWindow;
            this.Left = owner.Left + owner.ActualWidth  / 2 - 150;
            this.Top  = owner.Top  + owner.ActualHeight / 2 - 240;
        }


//        public CTecUtil.PrinterSettings PrinterSettings => _context.PrinterSettings;
        public PrintParameters PrintParams => _context.PrintParams;


        private PrintDialogWindowViewModel _context;
        private bool _isOpen;

        private void addShortcutKey(Button button)
        {
            var text = button.Content.ToString();

            if (text is null)
                return;

            if (text.Trim().TrimEnd('_').Contains('_') || text == CTecControls.Cultures.Resources.Menu_Exit)
            {
                string key;

                // find the underscore in the menu text (already guaranteed it's there and isn't last char)
                var ul = text.IndexOf("_");

                // underscore precedes the hotkey
                key = text.ToUpper(CultureInfo.CurrentCulture)[ul + 1].ToString();

                // add the key and its related action to the hotkeys
                var properties = new HotKeyList.KeyProperties(key, false, false, true);
                Action command = null;

                // is it one of the options that have hotkeys?
                if (text == Cultures.Resources.Option_Print)
                    command = new(() => printOrPreview(PrintActions.Print));
                else if (text == Cultures.Resources.Option_Preview)
                    command = new(() => printOrPreview(PrintActions.Preview));

                _context.HotKeys.Add(properties, command);

                //remove the underline from the button text
                button.Content = text.Remove(ul, 1);
            }
        }


        private void setSize()
        {
            this.Width  = grdOuter.Width  = grdInner.ActualWidth;
            this.Height = grdOuter.Height = grdInner.ActualHeight;
        }


        private void updateWindowParams(bool save = false) { if (_isOpen) _context.UpdateWindowParams(save); }


        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { updateWindowParams(); _context.Close(this); }


        private void window_SizeChanged(object sender, SizeChangedEventArgs e) => setSize();

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
                Close();
            else
                _context.CheckHotKey(e);
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


        private void window_Loaded(object sender, RoutedEventArgs e) => _isOpen = true;


        public CTecUtil.PrintActions PrintAction { get; private set; }


        internal void printOrPreview(CTecUtil.PrintActions action)
        {
            PrintAction = action;
            DialogResult = true;
        }


        private void PreviewButton_Click(object sender, RoutedEventArgs e) {  PrintAction = CTecUtil.PrintActions.Preview; DialogResult = true; }
        private void PrintButton_Click(object sender, RoutedEventArgs e) { PrintAction = CTecUtil.PrintActions.Print; DialogResult = true; }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => ClosePrint_Click(sender, e);
        private void ClosePrint_Click(object sender, EventArgs e) { PrintAction = CTecUtil.PrintActions.None; Close(); }


        private void cboPrinter_PreviewMouseDown(object sender, MouseButtonEventArgs e)   => _context.PrinterListIsOpen = true;
        private void PrintOptions_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.PrinterListIsOpen = false;
        private void PrinterList_MouseUp(object sender, MouseButtonEventArgs e)           => _context.PrinterListIsOpen = false;
        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e)   { try { DragMove(); updateWindowParams(); } catch { } }
    }
}
