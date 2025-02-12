using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CTecUtil.IO;

namespace Xfp.UI.Views
{
    public partial class ViewRevisionHistory : Window
    {
        public ViewRevisionHistory()
        {
            InitializeComponent();
            ReadDocx("/Resources/View Revision History.docx");
        }

        private void ReadDocx(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var flowDocumentConverter = new DocxToFlowDocumentConverter(stream);
                flowDocumentConverter.Read();
                fdr.Document = flowDocumentConverter.Document;
                this.Title = Path.GetFileName(path);
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseRevisionHistory_Click(object sender, EventArgs e)
        {

        }
    }
}
