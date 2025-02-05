using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
using CTecUtil.IO;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class EventLogViewer : Page
    {
        public EventLogViewer()
        {
            InitializeComponent();
            DataContext = _context = new EventLogViewerViewModel(this);
            _context.ClearText    = rtb.Document.Blocks.Clear;
            _context.DataReceived = new((string t) =>
                                    {
                                        rtb.AppendText(t);
                                        rtb.ScrollToEnd();
                                        rtb.UpdateLayout();
                                    });
        }


        public EventLogViewerViewModel _context;
        
        //public SerialComms.ConnectionStatus CommsStatus { get => _context.CommsStatus; set => _context.CommsStatus = value; }

        private void buttonStart_Click(object sender, RoutedEventArgs e) => _context.Start();
        private void buttonPause_Click(object sender, RoutedEventArgs e) => _context.Pause();
        private void buttonResume_Click(object sender, RoutedEventArgs e) => _context.Resume();
        private void buttonStop_Click(object sender, RoutedEventArgs e) => _context.Stop();
        //private void buttonReset_Click(object sender, RoutedEventArgs e)  => _context.ResetIndex();
        private void buttonSave_Click(object sender, RoutedEventArgs e)  => _context.SaveToFile();
        private void buttonOpen_Click(object sender, RoutedEventArgs e)  => _context.OpenFromFile();
        private void buttonClear_Click(object sender, RoutedEventArgs e) => _context.Clear();
    }
}
