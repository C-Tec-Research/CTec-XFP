using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace DocxReaderApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ReadDocx("Rendering Test.docx");
        }

        private void ReadDocx(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var flowDocumentConverter = new DocxToFlowDocumentConverter(stream);
                flowDocumentConverter.Read();
                this.flowDocumentReader.Document = flowDocumentConverter.Document;
                this.Title = Path.GetFileName(path);
            }
        }

        private void OnOpenFileClicked(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = ".docx",
                Filter = "Word documents (.docx)|*.docx"
            };

            if (openFileDialog.ShowDialog() == true)
                this.ReadDocx(openFileDialog.FileName);
        }

        private void OnAboutClicked(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
    }
}
