using System.Windows;
using System.Windows.Controls;
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
        private void buttonSave_Click(object sender, RoutedEventArgs e)  => _context.SaveToFile();
        private void buttonOpen_Click(object sender, RoutedEventArgs e)  => _context.OpenFromFile();
        private void buttonPrint_Click(object sender, RoutedEventArgs e)  => _context.Print();
        private void buttonClear_Click(object sender, RoutedEventArgs e) => _context.Clear();
    }
}
